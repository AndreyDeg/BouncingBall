using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Игровой уровень
public class LevelData
{
    GameObjectPull groundPull;
    GameObjectPull crystalPull;

    ILevelGenerator levelGenerator;
    ICrystalGenerator crystalGenerator;

    //Список блоков земли
    List<GameObject> ltGrounds = new List<GameObject>();
    //Список кристаллов
    List<GameObject> ltCrystals = new List<GameObject>();

    //Очки, заработанные за кристалы
    public int Score { get; private set; }

    public LevelData(GameObjectPull groundPull, GameObjectPull crystalPull)
    {
        this.groundPull = groundPull;
        this.crystalPull = crystalPull;
    }

    //Установить землю
    private void SetGround(Vector3 position)
    {
        var cube = groundPull.Get();
        cube.transform.position += position;
        ltGrounds.Add(cube);
    }

    //Установить кристалл
    private void SetCrystal(Vector3 position)
    {
        var crustal = crystalPull.Get();
        crustal.transform.position += position;
        ltCrystals.Add(crustal);
    }

    //Очистка уровня
    private void Clear()
    {
        Score = 0;

        foreach (var clone in ltGrounds)
            groundPull.Put(clone);
        ltGrounds.Clear();
        foreach (var clone in ltCrystals)
            crystalPull.Put(clone);
        ltCrystals.Clear();
    }

    public void Generate(ILevelGenerator levelGenerator, ICrystalGenerator crystalGenerator)
    {
        this.levelGenerator = levelGenerator;
        this.crystalGenerator = crystalGenerator;

        Clear();
        levelGenerator.Clear();

        GenerateNext();
    }

    private void GenerateNext()
    {
        foreach (var ground in levelGenerator.Generate())
        {
            for (int x = 0; x < ground.WidthX; x++)
                for (int z = 0; z < ground.WidthZ; z++)
                    SetGround(ground.Position - new Vector3(x, 0, z));

            if (ground.NumberLenght.HasValue && crystalGenerator.Check(ground.NumberLenght.Value))
                SetCrystal(ground.Position - new Vector3(Random.Range(0f, ground.WidthX - 1), 0, Random.Range(0f, ground.WidthZ - 1)));
        }
    }

    //Проверка, что в данной позиции есть земля
    public bool CheckGround(Vector3 pos)
    {
        int x = (int)Math.Round(pos.x);
        int z = (int)Math.Round(pos.z);

        //Генерация уровня дальше
        if (ltGrounds.Count < 15)
            GenerateNext();

        //Удаление блоков позади игрока
        foreach (var clone in ltGrounds)
            if (CheckFall(clone, pos))
                groundPull.Put(clone);
        ltGrounds.RemoveAll(clone => !clone.activeInHierarchy);

        //Подбор кристаллов
        foreach (var clone in ltCrystals)
            if (CheckFall(clone, pos) || CheckLoot(clone, pos))
                crystalPull.Put(clone);
        ltCrystals.RemoveAll(clone => !clone.activeInHierarchy);

        foreach (var groud in ltGrounds)
        {
            if (pos.y - 0.25 >= groud.transform.position.y || pos.y + 0.25 <= groud.transform.position.y )
                continue;
            if (pos.x - 0.25f >= groud.transform.position.x + 0.5f || pos.x + 0.25f <= groud.transform.position.x - 0.5f)
                continue;

            return true;
        }

        return false;
    }

    private bool CheckFall(GameObject gameObject, Vector3 pos)
    {
        var d = gameObject.transform.position.y - pos.y;
        if (d < -3)
            gameObject.transform.position -= new Vector3(0, 6, 0) * Time.deltaTime;

        return d < -5;
    }

    private bool CheckLoot(GameObject gameObject, Vector3 pos)
    {
        var d = (gameObject.transform.position - pos).magnitude;
        if (d < (0.5 + 0.25) / 2)
        {
            Score++;
            return true;
        }

        return false;
    }
}