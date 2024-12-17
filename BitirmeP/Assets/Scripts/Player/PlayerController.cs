using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hareketHýzý = 10;

    private int KalanZýplamaSayýsý;

    private float hareketInputYön;

    private Rigidbody2D rb;

    private Animator anim;

    private int ZýplamaSayýsý = 1;
    private int bakýlanYön = 1;

    private bool saðaDönük = true;
    private bool isWalking;
    private bool isGrounded;
    private bool Zýplayabilir;
    private bool DuvaraDeðiyor;
    private bool DuvardaKayýyor;
    private bool canFlip;
    private bool knockback;

    private float zýplamaGücü = 16.0f;
    public float ZeminKontrolüAlaný;
    public float DuvarKontrolMesafesi;
    public float duvardakaymaHýzý;
    public float havadaHareketGücü;
    public float havadirenciÇarpaný= 0.95f;
    public float zýplamaDeðiþkeniÇarpaný= 0.5f;
    public float duvardanZýplamaGücü; 
    public float duvarZýplamaGücü;
    public float knockbackStartTime;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private Vector2 knockbackSpeed;

    public LayerMask ZeminOlan;

    public Transform ZeminKontrolü;
    public Transform DuvarKontrolü;


    public Vector2 DuvardanZýplamaYönü;
    public Vector2 DuvarZýplamaYönü;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        KalanZýplamaSayýsý = ZýplamaSayýsý;
        DuvardanZýplamaYönü.Normalize();
        DuvarZýplamaYönü.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        hareketYönüKontrol();
        AnimasyonGüncelle();
        ZýplamayýKontrolEt();
        DuvardaKaymayýKontrolEt();
        CheckKnockBack();
    }

    private void FixedUpdate()
    {
        HareketiUygula();
        etrafýKontrolEt();
    }

    private void DuvardaKaymayýKontrolEt()
    {
        if(DuvaraDeðiyor && !isGrounded && rb.velocity.y < 0)
        {
            DuvardaKayýyor = true;
        }
        else
        {
            DuvardaKayýyor = false;
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

    private void etrafýKontrolEt()
    {
        isGrounded = Physics2D.OverlapCircle(ZeminKontrolü.position, ZeminKontrolüAlaný, ZeminOlan);
        DuvaraDeðiyor = Physics2D.Raycast(DuvarKontrolü.position, transform.right, DuvarKontrolMesafesi, ZeminOlan);
    }

    private void ZýplamayýKontrolEt()
    {
        if((isGrounded && rb.velocity.y <= 0) || DuvardaKayýyor) 
        {
            KalanZýplamaSayýsý = ZýplamaSayýsý;
        }
        if(KalanZýplamaSayýsý <= 0)
        {
            Zýplayabilir = false;
        }
        else
        {
            Zýplayabilir=true;
        }
    }

    public int GetFacingDirection()
    {
        return bakýlanYön;
    }

    private void hareketYönüKontrol()
    {
       if(saðaDönük && hareketInputYön < 0)
        {
            Çevir();
        }
       else if(!saðaDönük && hareketInputYön > 0)
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
        anim.SetBool("isWallSliding", DuvardaKayýyor);
    }

    private void CheckInput()
    {
        hareketInputYön = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            Zýpla();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * zýplamaDeðiþkeniÇarpaný );
        }
    }

    private void Zýpla()

    {
        if (Zýplayabilir)
        {
            rb.velocity = new Vector2(rb.velocity.x, zýplamaGücü);
            KalanZýplamaSayýsý--;
        }
        else if (DuvardaKayýyor && hareketInputYön ==0 && Zýplayabilir) 
        {
            DuvardaKayýyor = false;
            KalanZýplamaSayýsý--;
            Vector2 eklenecekGüç = new Vector2(duvardanZýplamaGücü * DuvardanZýplamaYönü.x * -bakýlanYön, duvardanZýplamaGücü * DuvardanZýplamaYönü.y);
            rb.AddForce(eklenecekGüç, ForceMode2D.Impulse);
        } 
        else if((DuvardaKayýyor || DuvaraDeðiyor)  && hareketInputYön !=0 && Zýplayabilir)
        {
            DuvardaKayýyor = false;
            KalanZýplamaSayýsý--;
            Vector2 eklenecekGüç = new Vector2(duvarZýplamaGücü * DuvarZýplamaYönü.x * hareketInputYön, duvarZýplamaGücü * DuvarZýplamaYönü.y);
            rb.AddForce(eklenecekGüç, ForceMode2D.Impulse);
        }

    }

    private void HareketiUygula()
    {
        if (isGrounded && !knockback)
        {
            rb.velocity = new Vector2(hareketHýzý * hareketInputYön, rb.velocity.y);
        }
        else if(!isGrounded && !DuvardaKayýyor && havadaHareketGücü !=0 ) 
        {
            Vector2 eklenecekForce = new Vector2(havadaHareketGücü * hareketInputYön, 0);
            rb.AddForce(eklenecekForce);


            if (Mathf.Abs(rb.velocity.x)> hareketHýzý && !knockback)
            {
                rb.velocity = new Vector2(hareketHýzý * hareketInputYön, rb.velocity.y);
            }
        }

        else if (!isGrounded && !DuvardaKayýyor && hareketInputYön == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * havadirenciÇarpaný,rb.velocity.y);
        }
       

       
        if (DuvardaKayýyor)
        {
            if(rb.velocity.y < -duvardakaymaHýzý) 
            {
                rb.velocity = new Vector2(rb.velocity.x, -duvardakaymaHýzý);
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
        if (!DuvardaKayýyor && canFlip &&!knockback)
        {
            bakýlanYön *= -1;
            saðaDönük = !saðaDönük;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ZeminKontrolü.position, ZeminKontrolüAlaný);
        Gizmos.DrawLine(DuvarKontrolü.position, new Vector3(DuvarKontrolü.position.x + DuvarKontrolMesafesi, DuvarKontrolü.position.y, DuvarKontrolü.position.z));
    }
}
