using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    //criação das variaveis
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;   
    public GameObject bulletPrefab;

    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float health = 100.0f;
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f;
    

    void Start()
    {
        //pega o componente navmesh do asset 
        agent = this.GetComponent<NavMeshAgent>();
        //faz ele ir diminuindo a velocidade quando estiver chegando no distino
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        //sempre faz o update da vida do npc
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {
        //posiciona a barra de vida de acordo com a camera
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        //seta o valor da vida como a variavel health
        healthBar.value = (int)health;
        //coloca a barra de vida em cima do npc
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    void UpdateHealth()
    {
        //se a vida for menos que 100 recupera ela
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {
        //caso colide com o projetil perca 10 de vida
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    //cria o metodo de ação da arvore de decisões de escolher uma posição aleatoria
    [Task]
    public void PickRandomDestination()
    {
        //cria a variavel que vai escolher uma posição aleatoria entre -100 e 100 nos eixo X e Z
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        //seta o destino dele para a posição escolhida aleatoriamente
        agent.SetDestination(dest);
        //fala que a ação foi concluida com sucesso
        Task.current.Succeed();
    }

    //cria o metodo de ação da arvore de decisões para se mover para a posição escolhida
    [Task]
    public void MoveToDestination()
    {
        //se estiver rodando a aplicação
        if (Task.isInspected)
        {
            //cira um timer para ver o tempo rodando
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        }
        //se estiver chegado no local
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            //fala que a ação foi concluida com sucesso
            Task.current.Succeed();
        }
    }
}

