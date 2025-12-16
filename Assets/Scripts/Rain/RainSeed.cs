using UnityEngine;

public class RandomCircleShaderController : MonoBehaviour
{
    public Material material;
    private float timer;
    private float refreshInterval = 0.5f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= refreshInterval)
        {
            float newSeed = Random.Range(0f, 1000f);
            material.SetFloat("_Seed", newSeed);

            timer = 0f;
        }
    }
}
