using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hareketH�z� = 10;

    private int KalanZ�plamaSay�s�;

    private float hareketInputY�n;

    private Rigidbody2D rb;

    private Animator anim;

    private int Z�plamaSay�s� = 1;
    private int bak�lanY�n = 1;

    private bool sa�aD�n�k = true;
    private bool isWalking;
    private bool isGrounded;
    private bool Z�playabilir;
    private bool DuvaraDe�iyor;
    private bool DuvardaKay�yor;
    private bool canFlip;
    private bool knockback;

    private float z�plamaG�c� = 16.0f;
    public float ZeminKontrol�Alan�;
    public float DuvarKontrolMesafesi;
    public float duvardakaymaH�z�;
    public float havadaHareketG�c�;
    public float havadirenci�arpan�= 0.95f;
    public float z�plamaDe�i�keni�arpan�= 0.5f;
    public float duvardanZ�plamaG�c�; 
    public float duvarZ�plamaG�c�;
    public float knockbackStartTime;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private Vector2 knockbackSpeed;

    public LayerMask ZeminOlan;

    public Transform ZeminKontrol�;
    public Transform DuvarKontrol�;


    public Vector2 DuvardanZ�plamaY�n�;
    public Vector2 DuvarZ�plamaY�n�;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        KalanZ�plamaSay�s� = Z�plamaSay�s�;
        DuvardanZ�plamaY�n�.Normalize();
        DuvarZ�plamaY�n�.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        hareketY�n�Kontrol();
        AnimasyonG�ncelle();
        Z�plamay�KontrolEt();
        DuvardaKaymay�KontrolEt();
        CheckKnockBack();
    }

    private void FixedUpdate()
    {
        HareketiUygula();
        etraf�KontrolEt();
    }

    private void DuvardaKaymay�KontrolEt()
    {
        if(DuvaraDe�iyor && !isGrounded && rb.velocity.y < 0)
        {
            DuvardaKay�yor = true;
        }
        else
        {
            DuvardaKay�yor = false;
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

    private void etraf�KontrolEt()
    {
        isGrounded = Physics2D.OverlapCircle(ZeminKontrol�.position, ZeminKontrol�Alan�, ZeminOlan);
        DuvaraDe�iyor = Physics2D.Raycast(DuvarKontrol�.position, transform.right, DuvarKontrolMesafesi, ZeminOlan);
    }

    private void Z�plamay�KontrolEt()
    {
        if((isGrounded && rb.velocity.y <= 0) || DuvardaKay�yor) 
        {
            KalanZ�plamaSay�s� = Z�plamaSay�s�;
        }
        if(KalanZ�plamaSay�s� <= 0)
        {
            Z�playabilir = false;
        }
        else
        {
            Z�playabilir=true;
        }
    }

    public int GetFacingDirection()
    {
        return bak�lanY�n;
    }

    private void hareketY�n�Kontrol()
    {
       if(sa�aD�n�k && hareketInputY�n < 0)
        {
            �evir();
        }
       else if(!sa�aD�n�k && hareketInputY�n > 0)
        {
            �evir();
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

    private void AnimasyonG�ncelle()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", DuvardaKay�yor);
    }

    private void CheckInput()
    {
        hareketInputY�n = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            Z�pla();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * z�plamaDe�i�keni�arpan� );
        }
    }

    private void Z�pla()

    {
        if (Z�playabilir)
        {
            rb.velocity = new Vector2(rb.velocity.x, z�plamaG�c�);
            KalanZ�plamaSay�s�--;
        }
        else if (DuvardaKay�yor && hareketInputY�n ==0 && Z�playabilir) 
        {
            DuvardaKay�yor = false;
            KalanZ�plamaSay�s�--;
            Vector2 eklenecekG�� = new Vector2(duvardanZ�plamaG�c� * DuvardanZ�plamaY�n�.x * -bak�lanY�n, duvardanZ�plamaG�c� * DuvardanZ�plamaY�n�.y);
            rb.AddForce(eklenecekG��, ForceMode2D.Impulse);
        } 
        else if((DuvardaKay�yor || DuvaraDe�iyor)  && hareketInputY�n !=0 && Z�playabilir)
        {
            DuvardaKay�yor = false;
            KalanZ�plamaSay�s�--;
            Vector2 eklenecekG�� = new Vector2(duvarZ�plamaG�c� * DuvarZ�plamaY�n�.x * hareketInputY�n, duvarZ�plamaG�c� * DuvarZ�plamaY�n�.y);
            rb.AddForce(eklenecekG��, ForceMode2D.Impulse);
        }

    }

    private void HareketiUygula()
    {
        if (isGrounded && !knockback)
        {
            rb.velocity = new Vector2(hareketH�z� * hareketInputY�n, rb.velocity.y);
        }
        else if(!isGrounded && !DuvardaKay�yor && havadaHareketG�c� !=0 ) 
        {
            Vector2 eklenecekForce = new Vector2(havadaHareketG�c� * hareketInputY�n, 0);
            rb.AddForce(eklenecekForce);


            if (Mathf.Abs(rb.velocity.x)> hareketH�z� && !knockback)
            {
                rb.velocity = new Vector2(hareketH�z� * hareketInputY�n, rb.velocity.y);
            }
        }

        else if (!isGrounded && !DuvardaKay�yor && hareketInputY�n == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * havadirenci�arpan�,rb.velocity.y);
        }
       

       
        if (DuvardaKay�yor)
        {
            if(rb.velocity.y < -duvardakaymaH�z�) 
            {
                rb.velocity = new Vector2(rb.velocity.x, -duvardakaymaH�z�);
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


    private void �evir ()
    {
        if (!DuvardaKay�yor && canFlip &&!knockback)
        {
            bak�lanY�n *= -1;
            sa�aD�n�k = !sa�aD�n�k;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ZeminKontrol�.position, ZeminKontrol�Alan�);
        Gizmos.DrawLine(DuvarKontrol�.position, new Vector3(DuvarKontrol�.position.x + DuvarKontrolMesafesi, DuvarKontrol�.position.y, DuvarKontrol�.position.z));
    }
}
