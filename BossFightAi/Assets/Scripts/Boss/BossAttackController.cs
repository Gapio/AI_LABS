using System;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [Serializable]
    public class NamedHitbox
    {
        public string name;
        public GameObject hitbox;
    }

    [SerializeField] NamedHitbox[] hitboxes;

    public void EnableHitbox(string hitboxName)
    {
        var hb = Find(hitboxName);
        if (hb) hb.SetActive(true);
    }

    public void DisableHitbox(string hitboxName)
    {
        var hb = Find(hitboxName);
        if (hb) hb.SetActive(false);
    }

    GameObject Find(string hitboxName)
    {
        for (int i = 0; i < hitboxes.Length; i++)
            if (hitboxes[i].name == hitboxName)
                return hitboxes[i].hitbox;
        return null;
    }
}
