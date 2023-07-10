using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isMine;
    public bool isFrozen;
    public bool isRevealed;

    public Texture2D mineTexture;
    public Texture2D[] numberTextures;
    public MeshRenderer meshRenderer;
    public Color highlightColor = Color.yellow;
    public float highlightTransparency = 0.5f;
    private Color originalColor;
    private float originalTransparency;
    public GameObject flagPrefab;
    private GameObject flag;
    private GameManager gameManager;
    public int x, y;
    public int minesAround;
    private AudioManager audioManager;

    public int MinesAround => minesAround;

    void Start()
    {
        originalColor = meshRenderer.material.color;
        originalTransparency = meshRenderer.material.color.a;
        gameManager = GameManager.instance;
        audioManager = AudioManager.instance;
    }

    void Update()
    {
        if (isFrozen || isRevealed) return;

        ProcessRightClick();
        ProcessLeftClick();
    }

    private void ProcessRightClick()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            ToggleFlag();
    }

    private void ProcessLeftClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform == transform && flag == null)
        {
            HandleFirstCellClick();
            HandleCellClick();
        }
    }

    private void HandleFirstCellClick()
    {
        if (!gameManager.isFirstReveal) return;

        gameManager.PlaceMines(this);
        gameManager.isFirstReveal = false;
    }

    private void HandleCellClick()
    {
        StartCoroutine(RevealCell());

        if (minesAround == 0 && !isMine)
            FreezeNeighbors();
    }

    private void ToggleFlag()
    {
        if (flag != null)
        {
            Destroy(flag);
            flag = null;
            audioManager.PlaySound("RemoveFlag");
        }
        else
        {
            flag = Instantiate(flagPrefab, transform.position, Quaternion.Euler(90, 0, 0), transform);
            audioManager.PlaySound("ToggleFlag");
        }
    }

    private void FreezeNeighbors()
    {
        List<Cell> neighbors = gameManager.GetNeighbors(this);
        foreach (Cell neighbor in neighbors)
        {
            if (neighbor.isMine) continue;

            neighbor.meshRenderer.material.color = Color.gray;
            neighbor.isFrozen = true;
        }
    }

    void OnMouseEnter()
    {
        // if (isFrozen) return;

        Color color = highlightColor;
        color.a = highlightTransparency;
        meshRenderer.material.color = color;
    }

    void OnMouseExit()
    {
        // if (isFrozen) return;

        Color color = originalColor;
        color.a = originalTransparency;
        meshRenderer.material.color = color;
    }

    public IEnumerator RevealCell()
    {
        for (int i = 0; i < 180; i += 10)
        {
            transform.Rotate(10, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        
        HandleCellReveal();
        isRevealed = true;
        gameManager.CheckWinCondition();
    }

    private void HandleCellReveal()
    {
        if (isMine)
        {
            meshRenderer.material.mainTexture = mineTexture;
            audioManager.PlaySound("MineExplosion");
            gameManager.ShowGameOver();
            FindObjectOfType<Timer>().StopTimer();
            gameManager.RevealAllCells();
        }
        else
        {
            meshRenderer.material.mainTexture = numberTextures[minesAround];
            audioManager.PlaySound("NumberReveal");
        }
    }
}
