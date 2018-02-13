using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flock State", menuName = "States/Flock")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FlockState : State {

    private NeighbourhoodTracker neighbourhood;
    private Rigidbody ownerRB;

    public void EnterState(Enemy owner)
    {
        neighbourhood = owner.GetComponent<NeighbourhoodTracker>();
        ownerRB = owner.RigidBody;
    }

    public override void Execute(Enemy owner)
    {
        //TODO: Implement direction using spawner / neighbourhood attributes, & owner navmesh
    }
}

