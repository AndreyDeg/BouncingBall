using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public float Speed = 2f;

    public GameObject sphere;
    public GameObject cube;
    public GameObject capsule;

    TextMesh scoreText;
    Camera mainCamera;
    Vector3 cameraPos;

    float maxY;
    Vector3 moveDirection;
    LevelData level;

    public void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMesh>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraPos = mainCamera.transform.position;

        var cubePull = new GameObjectPull(cube);
        var capsulePull = new GameObjectPull(capsule);
        level = new LevelData(cubePull, capsulePull);
        CreateLevel();
    }

    private void CreateLevel()
    {
        var levelGenerator = new PlatformGenerator(3);
        var crystalGenerator = new CrystalGeneratorRandom(5);
        level.Generate(levelGenerator, crystalGenerator);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (sphere.transform.position.y <= maxY - 10)
            {
                //Если шарик упал, то начать уровень заново
                maxY = 0;
                moveDirection = new Vector3(0, 0, 0);
                sphere.transform.position = new Vector3(0, 0.25f, 0);
                CreateLevel();
            }
        }

        //Направление движения
        moveDirection = new Vector3(0, moveDirection.y - 0.2f, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            moveDirection += new Vector3(-1, 0, 0) * Speed;
        if (Input.GetKey(KeyCode.RightArrow))
            moveDirection += new Vector3(1, 0, 0) * Speed;

        //Шарик движется
        sphere.transform.position += moveDirection * Time.deltaTime;

        scoreText.text = "Score: " + level.Score;

        //Проверка, что шарик на дороге
        if (level.CheckGround(sphere.transform.position))
        {
            if (moveDirection.y <= 0)
            {
                sphere.transform.position -= new Vector3(0, moveDirection.y, 0) * Time.deltaTime;
                moveDirection = new Vector3(0, 7f, 0);
                maxY = sphere.transform.position.y;
            }
            else
            {
                sphere.transform.position -= new Vector3(moveDirection.x, 0, 0) * Time.deltaTime * 2;
                if (level.CheckGround(sphere.transform.position))
                {
                    sphere.transform.position -= new Vector3(0, moveDirection.y, 0) * Time.deltaTime;
                    moveDirection = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {
            //Шарик упал
            if (sphere.transform.position.y < maxY - 10)
            {
                //Остановка шарика, чтоб не улетел в бесконечность
                moveDirection = new Vector3(0, 0, 0);
                scoreText.text = "Score: " + level.Score + "\nSpace to continue";
            }
        }
    }

    public void LateUpdate()
    {
        //камера двигается за шариком
        mainCamera.transform.position = sphere.transform.position + cameraPos;
    }
}
