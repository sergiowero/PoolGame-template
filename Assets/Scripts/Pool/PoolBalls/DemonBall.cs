using UnityEngine;
using System.Collections;

public class DemonBall : PoolBall
{
    public static Delegate1Args<IPlayer> GameOverWithPotted;


    public override void Potted(PocketIndexes pocketIndex)
    {
        base.Potted(pocketIndex);
        if (GameOverWithPotted != null)
            GameOverWithPotted(null);
    }

}
