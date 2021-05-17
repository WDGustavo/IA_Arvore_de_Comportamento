using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    //criação das variaveis
	float speed = 20.0F;
    float rotationSpeed = 120.0F;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    void Update() {
        //faz ele usar os inputs setado na vertical
        float translation = Input.GetAxis("Vertical") * speed;
        //faz ele usar os inputs setado na horizontal
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //faz o calculo pra ele se mover nas verticais com o deltaTime
        translation *= Time.deltaTime;
        //faz o calculo pra ele se mover nas horizontais com o deltaTime
        rotation *= Time.deltaTime;
        //fala o eixo que o translation vai interferir no caso o z
        transform.Translate(0, 0, translation);
        //fala o eixo que o rotation vai interferir no caso o y
        transform.Rotate(0, rotation, 0);
        //ao apertar a barra de espaço atira um projetiol
        if(Input.GetKeyDown("space"))
        {
            //instancia o projetil
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            //da a força de tiro do projetil
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
