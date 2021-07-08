using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterblockercolliderforEnemy : MonoBehaviour
{
    public MeshCollider characterCollider;
    public MeshCollider characterBlockerCollider;
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterBlockerCollider, true);

    }
}
