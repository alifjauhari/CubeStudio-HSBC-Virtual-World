using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow.Samples
{
    public class SpawnArrows : MonoBehaviour
    {
        public Transform target;

        void Start()
        {
            for (int i = 0; i < 30; i++)
            {
                Transform arrow = Instantiate(target,
                            new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0),
                            Quaternion.identity, target.parent);

                float scale = Random.Range(0.3f, 1f);
                arrow.localScale = new Vector3(scale, scale, 1);
            }
        }
    }
}