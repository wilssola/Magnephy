using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public bool Passed, Spawned;

    public void MoveCollider(float Height)
    {
        transform.localPosition = new Vector3(0, Height, 0);
    }

    private void OnTriggerExit(Collider Other)
    {
        // Se o Jogador já tiver passado perderá 1 Ponto.
        if (Other.tag == "Player" && Passed && Player.Score > 0)
        {
            Player.Score -= 1;

            Passed = !Passed;

            MoveCollider(1f);

            Player.SpawnPopUp("-1 " + Player.StaticPopUpPointText);

            // Debug.Log("Pontuação - Perdeu um ponto.");
        }
        // Se o Jogador não tiver passado ganhará 1 Ponto.
        else if (Other.tag == "Player" && !Passed)
        {
            Player.Score += 1;

            Passed = !Passed;

            MoveCollider(-1f);

            Player.SpawnPopUp("+1 " + Player.StaticPopUpPointText);

            // Debug.Log("Pontuação - Ganhou um ponto.");
        }
        // Spawnar novo Tile.
        if (!Spawned)
        {
            GameObject.Find("Ball").GetComponent<Player>().SpawnTile(transform.position.y, 21f, "TilesDynamic");

            Spawned = true;
        }
    }
}
