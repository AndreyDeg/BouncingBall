using System.Collections.Generic;
using UnityEngine;

// Создает платформы
public class PlatformGenerator : ILevelGenerator
{
    int roadLen; //длинна платформы
    int roadWidth; //ширина платформы
    int roadN; //общая длинна
    int roadYMin;
    int roadXMin;
    int roadXMax;

    public PlatformGenerator(int roadLen)
    {
        this.roadLen = roadLen;
        this.roadWidth = 1;
    }

    public void Clear()
    {
        roadN = 0;
        roadYMin = 0;
        roadXMin = 0;
        roadXMax = 0;
    }

    public IEnumerable<GroundData> Generate()
    {
        if (roadYMin == 0)
        {
            roadXMin = -2;
            roadXMax = 0;

            //Стартовая площадка
            yield return new GroundData(0, 0, 0, null, 3, 1);
        }

        for (int y = 0; y < 10; y++)
        {
            roadYMin++;
            if (Random.Range(0, 2) == 0)
            {
                //Платформа левее
                roadXMax = roadXMin - Random.Range(1, 3);
                roadXMin = roadXMax - Random.Range(1, roadLen + 1);
                for (int i = roadXMin; i <= roadXMax; i++)
                    yield return new GroundData(i, roadYMin * 1.5f, 0, roadN++, 1, roadWidth);
            }
            else
            {
                //Платформа правее
                roadXMin = roadXMax + Random.Range(1, 3);
                roadXMax = roadXMin + Random.Range(1, roadLen + 1);
                for (int i = roadXMin; i <= roadXMax; i++)
                    yield return new GroundData(i, roadYMin * 1.5f, 0, roadN++, 1, roadWidth);
            }
        }
    }
}