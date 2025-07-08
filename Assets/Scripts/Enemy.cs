using MyPlayer;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    [SerializeField] Animator anim;

    [Header("Movement")]
    [SerializeField] Transform flipPart;
    [SerializeField] float detectDistance = 10.0f;
    [SerializeField] float maxTravelDistance = 40.0f;
    [SerializeField] LayerMask detectionBlockMask;
    [SerializeField] float roamSpeed = 2.0f, roamInterval = 3.0f, roamRange = 5.0f, roamTimeout = 5.0f;
    [SerializeField] float chaseSpeed = 5.0f, maxDistance = 3.0f, minDistance = 2.0f, backstepSpeed = 1.0f, jumpPower = 15.0f;
    [SerializeField] float jumpYDifference = 2.0f;

    [Header("Ground Scanning")]
    [SerializeField] Vector2 groundScanOffset;
    [SerializeField] Vector2 groundScanBoxSize;
    [SerializeField] LayerMask groundScanMask;

    [Header("HP")]
    public float maxHp = 100.0f;
    public float hp { get; private set; }
    public bool dead { get; private set; } = false;

    bool isGrounded => Physics2D.OverlapBox((Vector2)transform.position + groundScanOffset, groundScanBoxSize, 0.0f, groundScanMask) != null;
    float playerDist => Vector2.Distance(player.transform.position, transform.position);

    Player player;
    Vector2 originPos;
    Rigidbody2D rb;
    TopLayer topLayer;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + groundScanOffset, groundScanBoxSize);

        Vector2 originPos = Application.isPlaying ? this.originPos : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(originPos, maxTravelDistance);
        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
        Gizmos.DrawLine(new Vector2(originPos.x - roamRange, originPos.y + 10.0f), new Vector2(originPos.x - roamRange, originPos.y - 10.0f));
        Gizmos.DrawLine(new Vector2(originPos.x + roamRange, originPos.y + 10.0f), new Vector2(originPos.x + roamRange, originPos.y - 10.0f));
        Gizmos.color = new Color(1.0f, 0.0f, 1.0f);
        Gizmos.DrawLine(new Vector2(transform.position.x - 10.0f, transform.position.y + jumpYDifference), new Vector2(transform.position.x + 10.0f, transform.position.y + jumpYDifference));
        Gizmos.DrawLine(new Vector2(transform.position.x - 10.0f, transform.position.y - jumpYDifference), new Vector2(transform.position.x + 10.0f, transform.position.y - jumpYDifference));
        if (Application.isPlaying)
        {
            topLayer.OnStateDrawGizmosSelected();
            Gizmos.color = IsPlayerDetectable() ? Color.green : new Color(0.5f, 0.0f, 0.0f);
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        originPos = transform.position;
        hp = maxHp;

        topLayer = new(this, null);
        topLayer.OnStateEnter();
    }
    private void Update()
    {
        topLayer.OnStateUpdate();
    }
    void DetectedUpdate()
    {

    }
    public bool GetDamage(float damage)
    {
        if (dead) return false;
        anim.SetTrigger("Damage");
        hp = Mathf.Max(hp - damage, 0.0f);
        if(hp <= 0)
        {
            anim.SetTrigger("Dead");
            dead = true;
        }
        return true;
    }
    bool IsPlayerDetectable()
    {
        return 
            Vector2.Distance(player.transform.position, originPos) < maxTravelDistance 
            && playerDist < detectDistance
            && !Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, Vector2.Distance(player.transform.position, transform.position), detectionBlockMask);
    }
    class TopLayer : TopLayer<Enemy>
    {
        public TopLayer(Enemy origin, FSMVals values) : base(origin, values)
        {
            defaultState = new Idle(origin, this);
            AddState("Idle", defaultState);
            AddState("Detected", new Detected(origin, this));
            AddState("Dead", new Dead(origin, this));
        }
        class Idle : Layer<Enemy>
        {
            public Idle(Enemy origin, Layer<Enemy> parent) : base(origin, parent)
            {
                defaultState = new WaitThen<Enemy>(origin, this, () => origin.roamInterval, () => ChangeState("Roaming"));
                AddState("Waiting", defaultState);
                AddState("Roaming", new Roaming(origin, this));
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.dead)
                {
                    parentLayer.ChangeState("Dead"); return;
                }
                if (origin.IsPlayerDetectable())
                {
                    parentLayer.ChangeState("Detected"); return;
                }
            }
            class Roaming : State<Enemy>
            {
                public Roaming(Enemy origin, Layer<Enemy> parent) : base(origin, parent) { }
                float counter = 0.0f;
                float roamDestinationX = 0.0f;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    counter = 0.0f;
                    roamDestinationX = origin.originPos.x + UnityEngine.Random.Range(-origin.roamRange, origin.roamRange);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if(origin.transform.position.x > roamDestinationX)
                    {
                        origin.rb.linearVelocityX = -origin.roamSpeed;
                        origin.flipPart.localScale = new Vector2(-1.0f, 1.0f);
                    }
                    if (origin.transform.position.x < roamDestinationX)
                    {
                        origin.rb.linearVelocityX = origin.roamSpeed;
                        origin.flipPart.localScale = new Vector2(1.0f, 1.0f);
                    }
                    if (Mathf.Abs(origin.transform.position.x - roamDestinationX) < 0.5f)
                    {
                        parentLayer.ChangeState("Waiting"); return;
                    }
                    counter += Time.deltaTime;
                    if(counter >= origin.roamTimeout)
                    {
                        parentLayer.ChangeState("Waiting"); return;
                    }
                }
            }
        }
        class Detected : Layer<Enemy>
        {
            public Detected(Enemy origin, Layer<Enemy> parent) : base(origin, parent)
            {
                defaultState = new Chasing(origin, this);
                AddState("Chasing", defaultState);
                AddState("InDist", new InDist(origin, this));
                AddState("Backstep", new Backstep(origin, this));
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.dead)
                {
                    parentLayer.ChangeState("Dead"); return;
                }
                if (!origin.IsPlayerDetectable())
                {
                    parentLayer.ChangeState("Idle"); return;
                }
                if (origin.player.transform.position.x < origin.transform.position.x)origin.flipPart.localScale = new Vector2(-1.0f, 1.0f);
                if (origin.player.transform.position.x > origin.transform.position.x) origin.flipPart.localScale = new Vector2(1.0f, 1.0f);
                if (origin.player.transform.position.y > origin.jumpYDifference + origin.transform.position.y && origin.isGrounded)
                {
                    origin.rb.linearVelocityY = origin.jumpPower;
                }
                origin.DetectedUpdate();
            }
            class Chasing : State<Enemy>
            {
                public Chasing(Enemy origin, Layer<Enemy> parent) : base(origin, parent) { }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (origin.player.transform.position.x < origin.transform.position.x) origin.rb.linearVelocityX = -origin.chaseSpeed;
                    if (origin.player.transform.position.x > origin.transform.position.x) origin.rb.linearVelocityX = origin.chaseSpeed;
                    if(origin.playerDist <= origin.maxDistance)
                    {
                        parentLayer.ChangeState("InDist"); return;
                    }
                }
            }
            class InDist : State<Enemy>
            {
                public InDist(Enemy origin, Layer<Enemy> parent) : base(origin, parent) { }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    origin.rb.linearVelocityX = 0.0f;
                    if (origin.playerDist > origin.maxDistance)
                    {
                        parentLayer.ChangeState("Chasing"); return;
                    }
                    if(origin.playerDist <= origin.minDistance)
                    {
                        parentLayer.ChangeState("Backstep"); return;
                    }
                }
            }
            class Backstep : State<Enemy>
            {
                public Backstep(Enemy origin, Layer<Enemy> parent) : base(origin, parent) { }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (origin.player.transform.position.x < origin.transform.position.x) origin.rb.linearVelocityX = origin.backstepSpeed;
                    if (origin.player.transform.position.x > origin.transform.position.x) origin.rb.linearVelocityX = -origin.backstepSpeed;
                    if (origin.playerDist > origin.minDistance)
                    {
                        parentLayer.ChangeState("InDist"); return;
                    }
                }
            }
        }
        class Dead : State<Enemy>
        {
            public Dead(Enemy origin, Layer<Enemy> parent) : base(origin, parent) { }
        }
    }
}