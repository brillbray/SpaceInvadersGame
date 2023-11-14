using System.Security.Cryptography;
using UnityEngine;

public class Invaders : MonoBehaviour
{
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[4];
    public AnimationCurve speed = new AnimationCurve();
    public Vector3 direction { get; private set; } = Vector3.right;
    public Vector3 initPosition { get; private set; }

    [Header("Grid")]
    public int rows = 4;
    public int columns = 8;

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;

    private void Awake()
    {
        initPosition = transform.position;

        CreateInvaderGrid();
    }

    private void CreateInvaderGrid()
    {
        for (int x = 0; x < rows; x++)
        {
            float width = 1.8f * (columns - 1);
            float height = 1.8f * (rows - 1);

            //substracting
            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);

            Vector3 rowPosition = new Vector3(centerOffset.x, (2f * x) + centerOffset.y, 0f);

            for (int y = 0; y < columns; y++)
            {
                //copas invader ke sebuah parent transform
                Invader invader = Instantiate(prefabs[x], transform);

                // kalkulasi dan penempatan invader pada row
                Vector3 position = rowPosition;
                position.x += 2.5f * y;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }

    private void MissileAttack()
    {
        int amountAlive = GetAliveCount();

        // Gada missile dipakai ketika tidak ada yang hidup
        if (amountAlive == 0) {
            return;
        }

        foreach (Transform invader in transform)
        {                 
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }
            //random spawn missile tergantung persentase amountAlive
            if (Random.value < (1f / (float)amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    private void Update()
    {
        int totalCount = rows * columns;
        int amountAlive = GetAliveCount();
        int amountKilled = totalCount - amountAlive;
        float percentKilled = (float)amountKilled / (float)totalCount;
    
        float speed = this.speed.Evaluate(percentKilled);
        transform.position = transform.position + direction * speed * Time.deltaTime;

        // check collision edge
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);


        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }

            // cek kiri atau kanan edge berdasar posisi sekarang
            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1f))
            {
                AdvancedRow();
                break;
            }
            else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1f))
            {
                AdvancedRow();
                break;
            }
        }
    }

    private void AdvancedRow()
    {
        direction = new Vector3(-direction.x, 0f, 0f);

        Vector3 position = transform.position;
        transform.position = position;
    }

    public void ResetInvaders()
    {
        direction = Vector3.right;
        transform.position = initPosition;

        foreach (Transform invader in transform) {
            invader.gameObject.SetActive(true);
        }
    }

    public int GetAliveCount()
    {
        int count = 0;

        foreach (Transform invader in transform)
        {
            if (invader.gameObject.activeSelf) {
                count++;
            }
        }
        return count;
    }
}
