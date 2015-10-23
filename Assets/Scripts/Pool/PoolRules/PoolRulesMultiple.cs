using UnityEngine;
using System.Collections;

public class PoolRulesMultiple : PoolRulesStandard
{
    protected override void CustomUpdate()
    {
        base.CustomUpdate();
        CurrentPlayer.PlayerUpdate();
    }
}
