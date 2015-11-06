using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CueAndGuidelines : MonoBehaviour {
    private Vector3 m_Pivot;
    private float m_CueOffset = 0;
    [SerializeField]
    [Range(0,2)]
    private float m_CuePullStrength;

    [System.Serializable]
    public class Line
    {
        [SerializeField]
        protected Vector3 offsetPositon;
        [SerializeField]
        protected Vector2 offsetSize = new Vector2(2, 2);
        [SerializeField]
        protected RectTransform line;
        [SerializeField]
        protected RectTransform outline;
        [SerializeField]
        protected RectTransform headPoint;
        [SerializeField]
        protected RectTransform tailPoint;
        [SerializeField]
        protected RectTransform headPoint2;
        [SerializeField]
        protected RectTransform tailPoint2;

        [SerializeField]
        public Image lineImage, outlineImage, headPointImage, tailPointImage, headPoint2Image, tailPoint2Image;

        public virtual void Hide()
        {
            if (line) line.gameObject.SetActive(false);
            if (outline) outline.gameObject.SetActive(false);
            if (headPoint) headPoint.gameObject.SetActive(false);
            if (tailPoint) tailPoint.gameObject.SetActive(false);
            if (headPoint2) headPoint2.gameObject.SetActive(false);
            if (tailPoint2) tailPoint2.gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            if (line) line.gameObject.SetActive(true);
            if (outline) outline.gameObject.SetActive(true);
            if (headPoint) headPoint.gameObject.SetActive(true);
            if (tailPoint) tailPoint.gameObject.SetActive(true);
            if (headPoint2) headPoint2.gameObject.SetActive(true);
            if (tailPoint2) tailPoint2.gameObject.SetActive(true);
        }

        public virtual void SetFade(float alpha)
        {
            SetFade(lineImage, alpha);
            SetFade(outlineImage, alpha);
            SetFade(headPointImage, alpha);
            SetFade(tailPointImage, alpha);
            SetFade(headPoint2Image, alpha);
            SetFade(tailPoint2Image, alpha);
        }

        protected void SetFade(Image image, float alpha)
        {
            if (!image)
                return;

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        private void RefreshAttachPoint()
        {
            if (headPoint)
                headPoint.position = line.position;
            if (tailPoint)
                tailPoint.position = line.parent.localToWorldMatrix.MultiplyPoint(line.localPosition + line.sizeDelta.x * line.right);
            if (headPoint2)
                headPoint2.position = outline.position;
            if(tailPoint2)
                tailPoint2.position = outline.parent.localToWorldMatrix.MultiplyPoint(outline.localPosition + outline.sizeDelta.x * outline.right);
        }

        public Transform parent
        {
            get { return line.parent; }
        }

        public virtual Vector3 direction
        {
            get { return line.right; }
            set
            {
                line.right = value;
                outline.right = value;
                RefreshAttachPoint();
            }
        }

        public virtual Vector3 position
        {
            get { return line.position; }
            set
            {
                line.position = value;
                outline.position = line.localToWorldMatrix.MultiplyPoint(offsetPositon);
                RefreshAttachPoint();
            }
        }

        public virtual bool active
        {
            get { return line.gameObject.activeInHierarchy; }
            set
            {
                line.gameObject.SetActive(value);
                outline.gameObject.SetActive(value);
            }
        }

        public virtual Vector2 size
        {
            get { return line.sizeDelta; }
            set
            {
                line.sizeDelta = value;
                outline.sizeDelta = line.sizeDelta + offsetSize;
                RefreshAttachPoint();
            }
        }
    }

    [System.Serializable]
    public class Circle : Line
    {
        [SerializeField]
        protected Vector2 offsetSize2 = new Vector2(-2,-2);
        [SerializeField]
        protected RectTransform outline2;

        [SerializeField]
        public Image outline2Image;

        public override void Hide()
        {
            base.Hide();
            if (outline2) outline2.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
            if (outline2) outline2.gameObject.SetActive(true);
        }

        public override void SetFade(float alpha)
        {
            base.SetFade(alpha);
            SetFade(outline2Image, alpha);
        }

        public override bool active
        {
            get
            {
                return base.active;
            }
            set
            {
                base.active = value;
                outline2.gameObject.SetActive(value);
            }
        }

        public override Vector3 direction
        {
            get
            {
                return base.direction;
            }
            set
            {
                base.direction = value;
                outline2.right = value;
            }
        }

        public override Vector3 position
        {
            get
            {
                return base.position;
            }
            set
            {
                base.position = value;
                outline2.position = outline.position;
            }
        }

        public override Vector2 size
        {
            get
            {
                return base.size;
            }
            set
            {
                base.size = value;
                outline2.sizeDelta = line.sizeDelta + offsetSize2;
            }
        }
    }

    [SerializeField]
    public Line m_Aim, m_CueBallDir, m_TargetBallDir;

    [SerializeField]
    public Circle m_CueBall;

    [SerializeField]
    private RectTransform m_Cue;
    [SerializeField]
    private RectTransform m_Forbidden;

    private Image m_ForbiddenImage;

    private bool m_bForbidden = false;

    [SerializeField]
    [Range(0,.03f)]
    private float m_GuidelineOffset;


    void Awake()
    {
        m_ForbiddenImage = m_Forbidden.GetComponent<Image>();
        m_Forbidden.gameObject.SetActive(false);
        gameObject.SetActive(false);
        m_Aim.direction = Vector3.down;
        FireSlider.OnSliderRelease += ReleaseCue;
        FireSlider.OnSliderValueChange += AdjustingChange;
        PoolRulesBase.onNewTurn += RoundBegin;
    }

    void OnDestroy()
    {
        FireSlider.OnSliderRelease -= ReleaseCue;
        FireSlider.OnSliderValueChange -= AdjustingChange;
        PoolRulesBase.onNewTurn -= RoundBegin;
    }

    public void AdjustingChange(float value)
    {
        m_CueOffset = value;
        Pools.Cue.SetPowerScalar(value);
    }

    private void RoundBegin(int round)
    {
        gameObject.SetActive(true);
    }

    public void ReleaseCue()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        float time = .05f;
        iTween.ValueTo(m_Cue.gameObject,
            iTween.Hash("from", m_CueOffset, 
                "to", 0,
                "time", time, 
                "onupdate", "MotionBlur", 
                "onupdatetarget", gameObject));
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        Pools.Cue.Fire();
    }

    private void MotionBlur(float value)
    {
        m_CueOffset = value;
    }

    public void SetFade(bool fade)
    {
        float alpha = 1;
        if (fade)
            alpha = .5f;
        
        if(m_ForbiddenImage)
        {
            Color c = m_ForbiddenImage.color;
            c.a = alpha;
            m_ForbiddenImage.color = c;
        }

        m_Aim.SetFade(alpha);
        m_CueBallDir.SetFade(alpha);
        m_TargetBallDir.SetFade(alpha);
        m_CueBall.SetFade(alpha);
    }

    public void GuidePointerAt(Vector3 hitPoint, Transform hitTrans, Vector3 hitNormal, Vector3 fireDir)
    {
        //gameObject.SetActive(true);

        Vector3 shp = MathTools.World2UI(hitPoint),//hit point
            shtp = MathTools.World2UI(hitTrans.position),//hitball position
            shwp = MathTools.World2UI(hitPoint + hitNormal.normalized * Pools.CueBall.GetRadius()),//whiteball position when hit
            swp = MathTools.World2UI(Pools.CueBall.GetPosition());//whiteball current position
        ////pivot
        m_Pivot = swp;
        //ball
        m_CueBall.position = shwp;
        m_Forbidden.position = shwp;
        //line
        m_Aim.position = shwp + fireDir * m_GuidelineOffset;
        Vector2 v1 = m_CueBall.parent.worldToLocalMatrix.MultiplyPoint(shwp);
        Vector2 v2 = m_CueBall.parent.worldToLocalMatrix.MultiplyPoint(swp);
        //m_AimLine.sizeDelta = new Vector2(Mathf.Max(Vector2.Distance(v1, v2) - Pools.CueBall.GetRadius(),1), m_AimLine.rect.height);
        m_Aim.size = new Vector2(Vector2.Distance(v1, v2), m_Aim.size.y);
        m_Aim.direction = (swp - shwp).normalized;
        //cue
        m_Cue.position = m_Pivot - m_Cue.right * m_CueOffset * m_CuePullStrength;
        m_Cue.right = -m_Aim.direction;
        if(hitTrans.CompareTag("Ball"))
        {
            m_TargetBallDir.active = !m_bForbidden;
            m_CueBallDir.active = !m_bForbidden;
            //hitballdir
            m_TargetBallDir.position = shtp;
            Vector3 v3 = (shtp - shp).normalized;
            float d = Mathf.Max(Vector2.Dot(-m_Aim.direction, v3), 0);
            float d40 = d * ConstantData.GuidelineLength;
            v3 = v3 * d;
            m_TargetBallDir.direction = v3;
            m_TargetBallDir.size = new Vector2(d40, m_TargetBallDir.size.y);
            //whiteballdir
            m_CueBallDir.position = shwp;
            m_CueBallDir.direction = -m_Aim.direction - v3;
            m_CueBallDir.size = new Vector2(ConstantData.GuidelineLength - d40, m_CueBallDir.size.y);
        }
        else
        {
            m_TargetBallDir.active = false;
            m_CueBallDir.active = false;
        }
    }

    public Vector3 GetPointerDirection()
    {
        return -m_Aim.direction;
    }

    public void Forbidden()
    {
        m_bForbidden = true;
        m_TargetBallDir.active = !m_bForbidden;
        m_CueBallDir.active = !m_bForbidden;
        m_Forbidden.gameObject.SetActive(true);
        m_CueBall.active = false;
    }

    public void Allow()
    {
        m_bForbidden = false;
        m_Forbidden.gameObject.SetActive(false);
        m_CueBall.active = true;
    }
}
