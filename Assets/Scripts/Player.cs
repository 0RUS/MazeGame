using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController characterController;
    public float speed = 5.0f;
    public float gravity = 20.0f;
    private bool shieldOn = false;
    private bool alive = true;
    public static Maze m;
    public static Vector3[] path;
    private double shield = 0;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.center = new Vector3(0.5f, 0.5f, 0.5f);
        characterController.radius = 1;
        StartCoroutine(AutoMove());
    }
    void Update()
    {
        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
        CheckZone();
    }
    private int Zones()
    {
        int xl = (int)Math.Ceiling((characterController.gameObject.transform.position.x - 0.5) / 5);
        int xr = (int)Math.Ceiling((characterController.gameObject.transform.position.x + 0.5) / 5);
        int zd = (int)Math.Ceiling((characterController.gameObject.transform.position.z - 0.5) / 5);
        int zu = (int)Math.Ceiling((characterController.gameObject.transform.position.z + 0.5) / 5);
        //Debug.Log(xl + " " + xr + " " + zd + " " + zu);
        if (m.cells[xl, zd].DeathZone || m.cells[xl, zu].DeathZone || m.cells[xr, zd].DeathZone || m.cells[xr, zu].DeathZone)
            return 1;
        else if (m.cells[xl, zd].Finish)
            return 2;
        else
            return 0;
    }

    private int Check()
    {
        int xl = (int)Math.Ceiling((characterController.gameObject.transform.position.x - 1.5) / 5);
        int xr = (int)Math.Ceiling((characterController.gameObject.transform.position.x + 1.5) / 5);
        int zd = (int)Math.Ceiling((characterController.gameObject.transform.position.z - 1.5) / 5);
        int zu = (int)Math.Ceiling((characterController.gameObject.transform.position.z + 1.5) / 5);
        //Debug.Log(xl + " " + xr + " " + zd + " " + zu);
        if (m.cells[xl, zd].DeathZone || m.cells[xl, zu].DeathZone || m.cells[xr, zd].DeathZone || m.cells[xr, zu].DeathZone)
            return 1;
        else if (m.cells[xl, zd].Finish)
            return 2;
        else
            return 0;
    }

    private void CheckZone()
    {
        int check = Zones();
        if (check == 1 && !shieldOn)
        {
            if (alive)
                StartCoroutine(Death());
            alive = false;
            return;
        }
        if (check == 2)
        {
            if (alive)
            {
                GameObject.Find("Player").transform.position = new Vector3(0, 2, 0);
                StartCoroutine(Finish());
                alive = false;
            }
        }
    }
    IEnumerator AutoMove()
    {
        yield return new WaitForSeconds(2);
        int i = 0;
        do
        {
            for (int j = 0; j < 20; j++)
            {
                if (Check() == 1 && !shieldOn)
                {
                    Shield(2);
                }
                else if ((Check() == 1 && shieldOn) || (Check() != 1 || (Zones() == 0 && shield >= 1)))
                {
                    moveDirection = new Vector3(path[path.Length - i - 1].x - transform.position.x, 0.0f, path[path.Length - i - 1].z - transform.position.z);
                    moveDirection *= speed;
                    characterController.Move(moveDirection * Time.deltaTime);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            i++;
        } while (i != path.Length);

    }
    public void Shield(float duration)
    {
        if (!shieldOn)
        {
            shieldOn = true;
            StartCoroutine(DesableShield(duration));
        }
    }
    IEnumerator DesableShield(float duration)
    {
        SetMaterial(GameObject.Find("Player"), "PlayerShield");
        shield = duration;
        while (shield > 0)
        {
            shield -= 0.1;
            yield return new WaitForSeconds(0.1f);
        }
        shieldOn = false;
        SetMaterial(GameObject.Find("Player"), "Player");
    }
    IEnumerator Death()
    {
        Debug.Log("DEATH");
        GameObject.Find("Player").GetComponent<Animation>().Play("Destroy");
        yield return new WaitForSeconds(2f);
        //Debug.Log("Restart");
        GameObject.Find("Player").transform.position = new Vector3(0, 2, 0);
        alive = true;
    }
    private void SetMaterial(GameObject x, string s)
    {
        foreach (Renderer r in x.GetComponentsInChildren<Renderer>())
        {
            r.material = Resources.Load($"Materials/{s}", typeof(Material)) as Material;
        }
    }
    IEnumerator Finish()
    {
        yield return new WaitForSeconds(0.4f);
        string[] results = AssetDatabase.FindAssets("Cell3D");
        MazeSpawner.Generate(Resources.Load<Cell>("Cell3D"), new Vector3(5, 0, 5));
        alive = true;
    }
}