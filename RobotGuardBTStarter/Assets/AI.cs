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
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update()
    {
        //posiciona a barra de vida de acordo com a camera
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        //seta o valor da vida como a variavel health
        healthBar.value = (int)health;
        //coloca a barra de vida em cima do npc
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth()
    {
        //se a vida for menos que 100 recupera ela
        if (health < 100)
            health++;
    }

    void OnCollisionEnter(Collision col)
    {
        //caso colide com o projetil perca 10 de vida
        if (col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }
    //*Wander.BT
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
    //*Fim do Wander.BT

    //*Patroll.BT
    //cria o metodo para ir para o destino para fazer a patrulha
    [Task]
    public void PickDestination(int x, int z)
    {
        //Cria a variavel para pegar as posições dos eixos x e z
        Vector3 dest = new Vector3(x, 0, z);
        //Seta o destino do NPC para a variavel dest
        agent.SetDestination(dest);
        //fala que a ação foi concluida com sucesso
        Task.current.Succeed();
    }
    //*Fim Patroll.BT

    //*Attack.BT
    [Task]
    //metodo boolenao para olhar para o player
    bool SeePlayer()
    {
        //fala que a distancia dele vai ser a distancia dele menos a do player
        Vector3 distance = player.transform.position - this.transform.position;
        //cria o raycast
        RaycastHit hit;
        //booleano para ver a parede
        bool seeWall = false;
        //debug para desenhar o raycast
        Debug.DrawRay(this.transform.position, distance, Color.red);
        //se o raycast tocar
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            //se o raycast colidir com um objeto com a tag wall seeWall fica true
            if (hit.collider.gameObject.tag == "wall")
            {
                //fica true
                seeWall = true;
            }
        }
        //se estiver rodando a aplicação
        if (Task.isInspected)
        {
            //mostra a distancia da wall
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        }
        //se a distancia dor menor que a distancia de visibilidade e caso seewall seja false retorne como true
        if (distance.magnitude < visibleRange && !seeWall)
        {
            //retorna como true
            return true;
        }
        //caso não retorne como falso
        else
        {
            //retorna como false
            return false;
        }
    }
    [Task]
    //metodo para ver o player
    public void TargetPlayer()
    {
        //pega a posição do player
        target = player.transform.position;
        //fala que a ação foi concluida com sucesso
        Task.current.Succeed();
    }

    [Task]
    //olha para o target
    public void LookAtTarget()
    {
        //cria a variavel para detectar a direção sendo menos que a posição do player
        Vector3 direction = target - this.transform.position;
        //faz a rotação ser mais suave
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        //se estiver rodando a aplicação
        if (Task.isInspected)
        {
            //mostra o angle com debug
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
        }
        //se for menor que 5 complete a task
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            //fala que a ação foi concluida com sucesso
            Task.current.Succeed();
        }
    }
    [Task]
    //metodo booleano para atirar
    public bool Fire()
    {
        //cria uma variavel para instanciar o projetil da bala
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        //da a força de movimentação para o projetil
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        //retorna como true
        return true;
    }
    //*Fim Attack.BT

    //*LookAround.BT
    [Task]
    //metodo booleano para olhar para o player
    bool Turn(float angle)
    {
        //variavel para pegar a posição e somar com o anglo para olhar para o target
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        //fala que o target é a variavel
        target = p;
        //retorna como true
        return true;
    }
    //*Fim LookAround.BT
    //*Death
    [Task]
    //metodo para verificar se a vida dele for menor que o valor escolhido
    public bool IsHealthLessThan(float health) 
    { 
        //retorna a vida dele para o float setado no metodo
        return this.health < health; 
    }
    [Task]
    //metodo para fazer o estado de morte
    public bool Explode() 
    { 
        //destroi a barra de vida
        Destroy(healthBar.gameObject); 
        //destroi o objeto que comtem o script
        Destroy(this.gameObject); 
        //retorna como true
        return true; 
    }
    //*Fim Death
}

