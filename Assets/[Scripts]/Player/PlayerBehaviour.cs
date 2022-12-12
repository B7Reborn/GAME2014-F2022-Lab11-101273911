using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement Properties")] 
    public float horizontalForce;
    public float horizontalSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint; // origin of circle
    public float groundRadius; // size of circle
    public LayerMask groundLayerMask; // stuff we can collide with
    public bool isGrounded;

    [Header("Animations")] 
    public Animator animator;
    public PlayerAnimationState playerAnimationState;

    [Header("Dust Trail Effect")] 
    public ParticleSystem dustTrail;
    public Color dustTrailColor;

    [Header("Screen Shake Properties")] 
    public CinemachineVirtualCamera playerCamera;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeIntensity;
    public float shakeDuration;
    public float shakeTimer;
    public bool isCameraShaking;

    [Header("Health System")] 
    public HealthBarController health;
    public LifeCounterController lives;
    public DeathPlaneController deathPlane;

    [Header("Controls")] 
    public Joystick leftStick;
    [Range(0.1f, 1.0f)]
    public float verticalThreshold;

    private Rigidbody2D rigidbody2D;
    private SoundManager soundManager;


    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = FindObjectOfType<PlayerHealth>().GetComponent<HealthBarController>();
        lives = FindObjectOfType<LifeCounterController>();
        deathPlane = FindObjectOfType<DeathPlaneController>();
        soundManager = FindObjectOfType<SoundManager>();
        leftStick = (Application.isMobilePlatform) ? GameObject.Find("Left Stick").GetComponent<Joystick>() : null;
        dustTrail = GetComponentInChildren<ParticleSystem>();

        playerCamera = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        perlin = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        isCameraShaking = false;
        shakeTimer = shakeDuration;
    }

    void Update()
    {
        if (health.value <= 0)
        {
            lives.LoseLife();

            if (lives.value > 0)
            {
                health.ResetHealth();
                deathPlane.Respawn(gameObject);
                soundManager.PlaySoundFX(Channel.PLAYER_DEATH_FX, Sound.DEATH);
            }
        }

        if (lives.value <= 0)
        {
            BulletManager.Instance().DestroyPool();
            SceneManager.LoadScene("End");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);

        Move();
        Jump();
        AirCheck();

        // Camera Shake Control
        if (isCameraShaking)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0.0f)
            {
                perlin.m_AmplitudeGain = 0.0f;
                shakeTimer = shakeDuration;
                isCameraShaking = false;
            }
        }
    }

    private void Move()
    {
        var x = Input.GetAxisRaw("Horizontal") + ((Application.isMobilePlatform) ? leftStick.Horizontal : 0.0f);

        if (x != 0.0f)
        {
            Flip(x);
            x = (x > 0.0) ? 1.0f : -1.0f; // sanitizing x

            rigidbody2D.AddForce(Vector2.right * x * horizontalForce * ((isGrounded) ? 1.0f : airFactor));

            var clampedXVelocity = Mathf.Clamp(rigidbody2D.velocity.x, -horizontalSpeed, horizontalSpeed);
            rigidbody2D.velocity = new Vector2(clampedXVelocity, rigidbody2D.velocity.y);

            ChangeAnimation(PlayerAnimationState.RUN);

            if (isGrounded)
            {
                CreateDustTrail();
            }
        }

        if ((isGrounded) && (x == 0.0f))
        {
            ChangeAnimation(PlayerAnimationState.IDLE);
        }
    }

    private void CreateDustTrail()
    {
        dustTrail.GetComponent<Renderer>().material.SetColor("_Color", dustTrailColor);
        dustTrail.Play();
    }

    private void ShakeCamera()
    {
        perlin.m_AmplitudeGain = shakeIntensity;
        isCameraShaking = true;
    }

    private void Jump()
    {
        var y = Input.GetAxis("Jump") + ((Application.isMobilePlatform) ? leftStick.Vertical : 0.0f);

        if (isGrounded && y > verticalThreshold)
        {
            rigidbody2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
            soundManager.PlaySoundFX(Channel.PLAYER_SOUND_FX, Sound.JUMP);
        }
    }

    private void AirCheck()
    {
        if (!isGrounded)
        {
            ChangeAnimation(PlayerAnimationState.JUMP);
        }
    }

    public void Flip(float x)
    {
        if (x != 0.0f)
        {
            transform.localScale = new Vector3((x > 0.0f) ? 1.0f : -1.0f, 1.0f, 1.0f);
        }
    }

    private void ChangeAnimation(PlayerAnimationState animationState)
    {
        playerAnimationState = animationState;
        animator.SetInteger("Animation State", (int)playerAnimationState);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(20);

            soundManager.PlaySoundFX(Channel.PLAYER_HURT_FX, Sound.HURT);
            ShakeCamera();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            health.TakeDamage(30);

            soundManager.PlaySoundFX(Channel.PLAYER_HURT_FX, Sound.HURT);
            ShakeCamera();
            
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            health.TakeDamage(10);

            soundManager.PlaySoundFX(Channel.PLAYER_HURT_FX, Sound.HURT);
            ShakeCamera();

        }
    }
}
