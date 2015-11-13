using UnityEngine;
using System.Collections;

public class DemonBall : PoolBall
{
    public static Delegate1Args<IPlayer> GameOverWithPotted;


    public override void Potted(PocketIndexes pocketIndex)
    {
        base.Potted(pocketIndex);

        if (GameManager.Rules.State == GlobalState.GAMEOVER)
            return;

        GameManager.Rules.State = GlobalState.GAMEOVER;

        if (GameOverWithPotted != null)
            GameOverWithPotted(null);
    }

}
