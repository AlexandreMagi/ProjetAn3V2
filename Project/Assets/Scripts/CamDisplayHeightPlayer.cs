using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamDisplayHeightPlayer : MonoBehaviour
{
    private const float playerHeight = 1.68f;
    private Vector2 playerWidth = new Vector2 (0.8f, 0.5f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position - Vector3.up * playerHeight / 2, new Vector3(playerWidth.x, playerHeight, playerWidth.y));
    }
}
