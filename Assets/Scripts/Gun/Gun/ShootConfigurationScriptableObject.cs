using System.Linq;
using Gun.Bullet;
using UnityEngine;

namespace Gun.Gun
{
 [CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/ShootConfig", order = 2)]
    public class ShootConfigurationScriptableObject : ScriptableObject
    {
        public LayerMask HitMask;
        public float FireRate = 0.25f;
        public float RecoilRecoverySpeed = 1f;
        public float MaxSpreadTime = 1f;
        public BulletSpreadType SpreadType = BulletSpreadType.Simple;
        [Header("Simple Spread")]
        public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
        public Vector3 MinSpread = Vector3.zero;
        [Header("Textured-Based Spread")]
        [Range(0.001f,5f)]
        public float SpreadMultiplier = 1f;
        public Texture2D SpreadTexture;
        
        
        public Vector3 GetSpread(float shooTime = 0 )
        {
            Vector3 spread = Vector3.zero;

            switch (SpreadType)
            {
                case BulletSpreadType.Simple:
                    spread = Vector3.Lerp(
                        new Vector3(
                            Random.Range(
                                -MinSpread.x,
                                MinSpread.x
                            ),
                            Random.Range(
                                -MinSpread.y,
                                MinSpread.y
                            ),
                            Random.Range(
                                -MinSpread.z,
                                MinSpread.z
                            )
                            ),
                        new Vector3(
                            Random.Range(
                                -Spread.x,
                                Spread.x
                            ),
                            Random.Range(
                                -Spread.y,
                                Spread.y
                            ),
                            Random.Range(
                                Spread.z,
                                Spread.z
                            )
                        ),
                        Mathf.Clamp01(shooTime / MaxSpreadTime)
                    );
                    break;
                
                case BulletSpreadType.TextureBased:
                    spread = GetTexturedDirection(shooTime);
                    spread *= SpreadMultiplier;
                    break;  
                case BulletSpreadType.None:
                    break;
            }
            
            return spread;
        }
        
        private Vector3 GetTexturedDirection(float shootTime)
        {
            Vector2 halfSize = new Vector2(SpreadTexture.width / 2f, SpreadTexture.height / 2f);
            int halfSquareExtents = Mathf.CeilToInt(
                Mathf.Lerp(
                    0.01f,
                    halfSize.x,
                    Mathf.Clamp01(shootTime / MaxSpreadTime)
                )
            );

            int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
            int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;
            Color[] sampleColors = SpreadTexture.GetPixels(
                minX,
                minY,
                halfSquareExtents * 2,
                halfSquareExtents * 2
                );
           float[] colorsAsGray = System.Array.ConvertAll(sampleColors,(color) => color.grayscale);
           float totalGreyValue = colorsAsGray.Sum();
           float randomValue = Random.Range(0, totalGreyValue);
           int i = 0;
           for (; i < colorsAsGray.Length; i++)
           {
               randomValue -= colorsAsGray[i];
               if (randomValue <= 0)
               {
                    break;
               }
           }
           int x = minX + i % (halfSquareExtents * 2);
           int y = minY + i / (halfSquareExtents * 2);
              
           Vector2 targetPosition = new Vector2(x, y);
           Vector2 direction = (targetPosition - halfSize) / halfSize.x; 
           return direction;
           
        }
       
    }
}
