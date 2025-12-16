using UnityEngine;

namespace Gun.Audio
{
    [CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 5)]
    public class AudioScriptableObject : ScriptableObject
    {
        [Range(0,1f)]
        public float Volume = 1f;
        public AudioClip[] FireClips;
        public AudioClip ReloadClip;
        public AudioClip EmptyClip;
        public AudioClip LastBulletClip;
        
        public void PlayShootingClip(AudioSource source,bool IsLastBullet)
        {
            if (IsLastBullet && LastBulletClip != null)
            {
                source.PlayOneShot(LastBulletClip, Volume);
            }
            else
            {
                source.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
            }
        }
        public void PlayOutOfAmmoClip(AudioSource source) 
        {
                if (EmptyClip != null)
                {
                    // make sure to play the empty clip only once
                    if (!source.isPlaying)
                    {
                        source.PlayOneShot(EmptyClip, Volume);
                    }
                }
        }
        
        public void PlayReloadClip(AudioSource source)
        {
            if (ReloadClip != null)
            {
                source.PlayOneShot(ReloadClip, Volume);
            }
        }
         
    }
   
}