using System.Collections;
using System.Text;

public class ConstantData
{
    public const float BallRadiusAdjustment = .004f;

    public const float BallDragRayCastRadius = 1f;

    public const float AdjustingCueScalar = 0.3f;

    public const float TurnWaitTime = 1.5f;

    public const float TimePerRoundLow = 30;

    public const float TimePerRoundHigh = 60;

    public const float TimeLimitQuickFire = 60 * 2;

    private static PoolDataAsset PoolDatas = null;
    public static string PoolDataAssetsFile { get { return "PoolEnvironmentData/PoolPhysical.asset"; } }

    public static PoolDataAsset GetPoolDatas()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        if (PoolDatas == null)
        {
            PoolDatas = StreamTools.DeserializeObject<PoolDataAsset>(PoolDataAssetsFile);
            if (PoolDatas == null)
            {
                UnityEngine.Debug.Log("Pool asset data is null");
                PoolDatas = new PoolDataAsset();
            }
        }
#endif
        return PoolDatas;
    }

#if UNITY_ANDROID
    public static void SetPoolDatas(PoolDataAsset data)
    {
        PoolDatas = data;
    }
#endif
}

[System.Serializable]
public class PoolDataAsset
{
    public PoolDataAsset()
    {
        MaxImpulse = 60;
        BallAngularDrag = 6;
        BallDrag = .5f;
        BallBounciness = .95f;
        RailBounciness = .8f;
        HorizontalSidingStrength = .5f;
        VerticalSidingStrength = 1;
    }

    public float MaxImpulse;

    public float BallAngularDrag;

    public float BallDrag;

    public float BallBounciness;

    public float RailBounciness;

    public float HorizontalSidingStrength;

    public float VerticalSidingStrength;

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Max impulse : ").Append(MaxImpulse)
            .Append("\nWhite ball angular drag : ").Append(BallAngularDrag)
            .Append("\nWhite ball drag : ").Append(BallDrag)
            .Append("\nBall bounciness : ").Append(BallBounciness)
            .Append("\nRail bounciness : ").Append(RailBounciness)
            .Append("\nHorizontal siding strength : ").Append(HorizontalSidingStrength)
            .Append("\nVertical siding strength : ").Append(VerticalSidingStrength);
        return builder.ToString();
    }
}

