using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hareketHızı = 10;

    private int KalanZıplamaSayısı;

    private float hareketInputYön;

    private Rigidbody2D rb;

    private Animator anim;

    private int ZıplamaSayısı = 1;
    private int bakılanYön = 1;

    private bool sağaDönük = true;
    private bool isWalking;
    private bool isGrounded;
    private bool Zıplayabilir;
    private bool DuvaraDeğiyor;
    private bool DuvardaKayıyor;
    private bool canFlip;
    private bool knockback;

    private float zıplamaGücü = 16.0f;
    public float ZeminKontrolüAlanı;
    public float DuvarKontrolMesafesi;
    public float duvardakaymaHızı;
    public float havadaHareketGücü;
    public float havadirenciÇarpanı= 0.95f;
    public float zıplamaDeğişkeniÇarpanı= 0.5f;
    public float duvardanZıplamaGücü; 
    public float duvarZıplamaGücü;
    public float knockbackStartTime;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private Vector2 knockbackSpeed;

    public LayerMask ZeminOlan;

    public Transform ZeminKontrolü;
    public Transform DuvarKontrolü;


    public Vector2 DuvardanZıplamaYönü;
    public Vector2 DuvarZıplamaYönü;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        KalanZıplamaSayısı = ZıplamaSayısı;
        DuvardanZıplamaYönü.Normalize();
        DuvarZıplamaYönü.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        hareketYönüKontrol();
        AnimasyonGüncelle();
        ZıplamayıKontrolEt();
        DuvardaKaymayıKontrolEt();
        CheckKnockBack();
    }

    private void FixedUpdate()
    {
        HareketiUygula();
        etrafıKontrolEt();
    }

    private void DuvardaKaymayıKontrolEt()
    {
        if(DuvaraDeğiyor && !isGrounded && rb.velocity.y < 0)
        {
            DuvardaKayıyor = true;
        }
        else
        {
            DuvardaKayıyor = false;
        }
    }

    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);

    }

    private void CheckKnockBack()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback=false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);  
        }
    }

    private void etrafıKontrolEt()
    {
        isGrounded = Physics2D.OverlapCircle(ZeminKontrolü.position, ZeminKontrolüAlanı, ZeminOlan);
        DuvaraDeğiyor = Physics2D.Raycast(DuvarKontrolü.position, transform.right, DuvarKontrolMesafesi, ZeminOlan);
    }

    private void ZıplamayıKontrolEt()
    {
        if((isGrounded && rb.velocity.y <= 0) || DuvardaKayıyor) 
        {
            KalanZıplamaSayısı = ZıplamaSayısı;
        }
        if(KalanZıplamaSayısı <= 0)
        {
            Zıplayabilir = false;
        }
        else
        {
            Zıplayabilir=true;
        }
    }

    public int GetFacingDirection()
    {
        return bakılanYön;
    }

    private void hareketYönüKontrol()
    {
       if(sağaDönük && hareketInputYön < 0)
        {
            Çevir();
        }
       else if(!sağaDönük && hareketInputYön > 0)
        {
            Çevir();
        }


       if(rb.velocity.x !=0)
        {
            isWalking = true;

        }
        else
        {
            isWalking = false;
        }
    }

    private void AnimasyonGüncelle()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", DuvardaKayıyor);
    }

    private void CheckInput()
    {
        hareketInputYön = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            Zıpla();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * zıplamaDeğişkeniÇarpanı );
        }
    }

    private void Zıpla()

    {
        if (Zıplayabilir)
        {
            rb.velocity = new Vector2(rb.velocity.x, zıplamaGücü);
            KalanZıplamaSayısı--;
        }
        else if (DuvardaKayıyor && hareketInputYön ==0 && Zıplayabilir) 
        {
            DuvardaKayıyor = false;
            KalanZıplamaSayısı--;
            Vector2 eklenecekGüç = new Vector2(duvardanZıplamaGücü * DuvardanZıplamaYönü.x * -bakılanYön, duvardanZıplamaGücü * DuvardanZıplamaYönü.y);
            rb.AddForce(eklenecekGüç, ForceMode2D.Impulse);
        } 
        else if((DuvardaKayıyor || DuvaraDeğiyor)  && hareketInputYön !=0 && Zıplayabilir)
        {
            DuvardaKayıyor = false;
            KalanZıplamaSayısı--;
            Vector2 eklenecekGüç = new Vector2(duvarZıplamaGücü * DuvarZıplamaYönü.x * hareketInputYön, duvarZıplamaGücü * DuvarZıplamaYönü.y);
            rb.AddForce(eklenecekGüç, ForceMode2D.Impulse);
        }

    }

    private void HareketiUygula()
    {
        if (isGrounded && !knockback)
        {
            rb.velocity = new Vector2(hareketHızı * hareketInputYön, rb.velocity.y);
        }
        else if(!isGrounded && !DuvardaKayıyor && havadaHareketGücü !=0 ) 
        {
            Vector2 eklenecekForce = new Vector2(havadaHareketGücü * hareketInputYön, 0);
            rb.AddForce(eklenecekForce);


            if (Mathf.Abs(rb.velocity.x)> hareketHızı && !knockback)
            {
                rb.velocity = new Vector2(hareketHızı * hareketInputYön, rb.velocity.y);
            }
        }

        else if (!isGrounded && !DuvardaKayıyor && hareketInputYön == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * havadirenciÇarpanı,rb.velocity.y);
        }
       

       
        if (DuvardaKayıyor)
        {
            if(rb.velocity.y < -duvardakaymaHızı) 
            {
                rb.velocity = new Vector2(rb.velocity.x, -duvardakaymaHızı);
            }
        }
    }

    public void DisableFlip()
    {
       canFlip= false;
    }

    public void EnableFlip()
    {
        canFlip= true;
    }


    private void Çevir ()
    {
        if (!DuvardaKayıyor && canFlip &&!knockback)
        {
            bakılanYön *= -1;
            sağaDönük = !sağaDönük;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ZeminKontrolü.position, ZeminKontrolüAlanı);
        Gizmos.DrawLine(DuvarKontrolü.position, new Vector3(DuvarKontrolü.position.x + DuvarKontrolMesafesi, DuvarKontrolü.position.y, DuvarKontrolü.position.z));
    }
}
