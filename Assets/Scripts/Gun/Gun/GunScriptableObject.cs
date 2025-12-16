using System.Collections;
using Gun.Audio;
using Gun.Bullet;
using Gun.Fx;
using Interfaces;
using Inventory;
using MapEvents;
using UnityEngine;
using UnityEngine.Pool;

namespace Gun.Gun
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject
    {
        public GunType Type;
        public string Name;
        public GameObject ModelPrefab;
        public Sprite GunIcon;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;

        public DamageConfigScriptableObject DamageConfig;
        public ShootConfigurationScriptableObject ShootConfig;
        public TrailConfigScriptableObject TrailConfig;
        public AmmoScriptableObject AmmoConfig;
        public AudioScriptableObject AudioConfig;

        private MonoBehaviour _ActiveMonoBehaviour;
        private GameObject _Model;
        private AudioSource _ShootAudioSource;
        private Animator gunAnimator;
        private float _LastShootTime;
        private float _InitialClickTime;
        private float _StopShootingTime;
        private bool _LastFrameWantedToShoot;
        private ParticleSystem _ShootSystem;
        private ObjectPool<TrailRenderer> _TrailPool;
        public bool IsReloading = false;
        private bool _IsReloadSoundPlaying = false;

        public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
        {
            this._ActiveMonoBehaviour = ActiveMonoBehaviour;
            _LastShootTime = 0; //  will not be properly reset in the editor but fine at build time
            _TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
            _Model = Instantiate(ModelPrefab);
            _Model.transform.SetParent(Parent, false);
            _Model.transform.localPosition = SpawnPoint;
            _Model.transform.localRotation = Quaternion.Euler(SpawnRotation);
            // get the animator component
            gunAnimator = _Model.GetComponent<Animator>();
            _ShootSystem = _Model.GetComponentInChildren<ParticleSystem>();
            _ShootAudioSource = _Model.GetComponentInChildren<AudioSource>();
        }

        public void Despawn()
        {
            Destroy(_Model);
        }

        public void TryToShoot()
        {
            InventoryManager inventoryManager = FindAnyObjectByType<InventoryManager>();

            if (Time.time - _LastShootTime - ShootConfig.FireRate > Time.deltaTime)
            {
                float lastDuration = Mathf.Clamp(
                    0,
                    (_StopShootingTime - _InitialClickTime),
                    ShootConfig.MaxSpreadTime
                );
                float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - _StopShootingTime))
                                 / ShootConfig.RecoilRecoverySpeed;

                _InitialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
            }

            if (Time.time > ShootConfig.FireRate + _LastShootTime)
            {
                if (AmmoConfig.CurrentClipAmmo == 0)
                {
                    AudioConfig.PlayOutOfAmmoClip(_ShootAudioSource);
                    gunAnimator.Play("Empty");
                    return;
                }

                _LastShootTime = Time.time;
                _ShootSystem.Play();
                gunAnimator.Play("Shoot");
                // when animation finishes, reset the animator
                gunAnimator.Play("Idle");
                AudioConfig.PlayShootingClip(_ShootAudioSource, AmmoConfig.CurrentClipAmmo == 1);

                Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - _InitialClickTime);
                _Model.transform.forward += _Model.transform.TransformDirection(spreadAmount);

                Vector3 shootDirection = _Model.transform.forward;

                AmmoConfig.CurrentClipAmmo--;

                if (Physics.Raycast(
                        _ShootSystem.transform.position,
                        shootDirection,
                        out RaycastHit hit,
                        float.MaxValue,
                        ShootConfig.HitMask
                    ))
                {
                    _ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            _ShootSystem.transform.position,
                            hit.point,
                            hit
                        )
                    );
                }
                else
                {
                    _ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            _ShootSystem.transform.position,
                            _ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                            new RaycastHit()
                        )
                    );
                }
            }
            inventoryManager.RefreshGuns();
            // show raycast in game view for debug
            Debug.DrawRay(_ShootSystem.transform.position, _Model.transform.forward * TrailConfig.MissDistance,
                Color.red, 10f);
        }

        public bool CanReload()
        {
            return AmmoConfig.CanReload();
        }

        /// <summary>
        /// Play the reload sound
        /// </summary>
        public void StartReload()
        {
            Debug.Log("StartReload");
            if (_IsReloadSoundPlaying && name != "Glock" || IsReloading )
            {
                Debug.Log("reload sound already playing");
                return;
            }
            
            IsReloading = true;
            Debug.Log("glock reload animation started");
            gunAnimator.Play("Reload");
            
            _IsReloadSoundPlaying = true;
            AudioConfig.PlayReloadClip(_ShootAudioSource);

    
            if (AudioConfig.ReloadClip.length > 0)
            {
                // Wait for the reload clip to finish playing
                _ActiveMonoBehaviour.StartCoroutine(WaitForReloadSound());
            }
          
        }

        private IEnumerator WaitForReloadSound()
        {
            yield return new WaitForSeconds(AudioConfig.ReloadClip.length);
            Debug.Log("glock reload animation finished");
            IsReloading = false;
            AmmoConfig.Reload();
            _IsReloadSoundPlaying = false;
        }

        public void Tick(bool wantsToShoot)
        {
            if (_Model != null)
            {
                _Model.transform.localRotation = Quaternion.Lerp(
                    _Model.transform.localRotation,
                    Quaternion.Euler(SpawnRotation),
                    Time.deltaTime * ShootConfig.RecoilRecoverySpeed
                );


                if (wantsToShoot)
                {
                    _LastFrameWantedToShoot = true;
                    TryToShoot();
                }
                else if (!wantsToShoot && _LastFrameWantedToShoot)
                {
                    _StopShootingTime = Time.time;
                    _LastFrameWantedToShoot = false;
                }
            }
        }

        private IEnumerator PlayTrail(Vector3 start, Vector3 end, RaycastHit hit)
        {
            TrailRenderer instance = _TrailPool.Get();
            instance.gameObject.SetActive(true);
            instance.transform.position = start;
            yield return null; //avoid position carry over from last frame if reused

            instance.emitting = true;

            float distance = Vector3.Distance(start, end);
            float remainingDistance = distance;
            while (remainingDistance > 0)
            {
                instance.transform.position = Vector3.Lerp(
                    start,
                    end,
                    Mathf.Clamp(1 - (remainingDistance / distance), 0, 1)
                );
                remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

                yield return null;
            }

            instance.transform.position = end;

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    Transform currentObject = hit.transform;
                    while (currentObject.TryGetComponent(out IDamageable damageableComponent) == false) { 
                        currentObject = currentObject.parent;
                    }
                    currentObject.GetComponentInParent<IDamageable>()?.TakeDamage(DamageConfig.GetDamage());
                }

                if (hit.collider.TryGetComponent(out MoveUfo component))
                {
                    component.IncreaseMadness();
                }
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            instance.emitting = false;
            instance.gameObject.SetActive(false);
            _TrailPool.Release(instance);
        }

        private TrailRenderer CreateTrail()
        {
            GameObject instance = new GameObject("Bullet Trail");
            TrailRenderer trail = instance.AddComponent<TrailRenderer>();
            trail.colorGradient = TrailConfig.Color;
            trail.material = TrailConfig.Material;
            trail.widthCurve = TrailConfig.WidthCurve;
            trail.time = TrailConfig.Duration;
            trail.minVertexDistance = TrailConfig.MinVertexDistance;

            trail.emitting = false;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return trail;
        }
    }
}