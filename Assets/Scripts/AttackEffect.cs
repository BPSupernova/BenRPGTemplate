using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public float effectDuration;
    public int soundEffect;

    private void Start() {
        AudioManager.instance.PlaySFX(soundEffect);
    }
    
    void Update()
    {
        Destroy(gameObject, effectDuration);
    }
}
