using UnityEngine;
using TMPro;

public class ControllerFoguete : MonoBehaviour
{
    // Componentes Públicos
    public float lancadoForce = 100f;
    public float rotationSpeed = -10f;
    public float inclinacaoRotacao = 30f;
    public float speed;
    public float ejetarCompartimentoAltura = 100f;
    public float paraquedaAltura = 100f;
    public GameObject ejetarCompartimentoObjeto;
    public GameObject paraquedas;
    public TextMeshProUGUI alturaText;
    public TextMeshProUGUI velocidadeText;
    public TextMeshProUGUI tempText;
    public AudioClip voarSom;
    public AudioClip paraquedasSom;

    // Componentes Privados
    private bool lancado = false;
    private Rigidbody fogueteRb;
    private float inicioTime;
    private bool compartimentoEjetado = false;
    private bool paraquedasAtivado = false;
    private float inclinationAngle = 30f;
    private float nextWindTime = 10f;
    private float windInterval = 2f;
    private float windForce;
    private AudioSource playerAudio;

    // Start is called before the first frame update
    void Start()
    {
        // Iniciando com o Rigidybody do foguete, iniciando o tempo e ativando e desativando objetos
        fogueteRb = GetComponent<Rigidbody>();
        inicioTime = Time.time;
        ejetarCompartimentoObjeto.SetActive(true); //o compartimento vai iniciar ativo
        paraquedas.SetActive(false); // O paraquedas inicia desativado
        playerAudio = GetComponent<AudioSource>(); // Pegando o componente do Audio
    }

    // Update is called once per frame
    void Update()
    {
        // Ação ao apertar em espaço
        if (Input.GetKeyDown(KeyCode.Space) && !lancado)
        {
            LaunchRocket();
        }
    }

    // Lançamento do foguete e som
    void LaunchRocket()
    {
        lancado = true;

        fogueteRb.AddForce(transform.up * lancadoForce, ForceMode.Impulse);

        transform.rotation = Quaternion.Euler(Vector3.right * inclinationAngle); // Tentativa de força como o vento

        nextWindTime = Time.time + windInterval; // Tentativa de força como o vento

        playerAudio.PlayOneShot(voarSom, 1.0f); // Som da partida
    }

    void FixedUpdate()
    {
        // Caso seja lançado o compartimento é ejetado, o paraquedas ativado e a força adicional do vento
        if (lancado)
        {
            UpdateUI();
            
            if (transform.position.y >= ejetarCompartimentoAltura && !compartimentoEjetado)
            {
                EjectCompartment();
            }

            if (transform.position.y >= paraquedaAltura && !paraquedasAtivado)
            {
                DeployParachute();
            }

            if (Time.time >= nextWindTime)
            {
                ApplyWindForce();
                nextWindTime = Time.time + windInterval;
            }
        }
    }

    // Pegando a altura, velocidade e tempo em texto
    void UpdateUI()
    {
        float height = transform.position.y;
        float velocity = fogueteRb.velocity.magnitude;
        float currentTime = Time.time - inicioTime;

        alturaText.text = "Altura: " + height.ToString("F2") + " metros";
        velocidadeText.text = "Velocidade: " + velocity.ToString("F2") + " m/s";
        tempText.text = "Tempo: " + currentTime.ToString("F2") + " segundos";
    }

    // Tentativa de adição de força como o vento
    void ApplyWindForce()
    {
        Vector3 wind = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * windForce;

        fogueteRb.AddForce(wind, ForceMode.Force);
    }

    // Ejetando compartimento
    void EjectCompartment()
    {
        ejetarCompartimentoObjeto.SetActive(false);
        compartimentoEjetado = true;
    }

    // Ativando o paraquedas
    void DeployParachute()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            paraquedas.SetActive(true);

            fogueteRb.useGravity = true;
            Physics.gravity /= 10f;
            fogueteRb.drag = 2f;
            fogueteRb.angularDrag = 0.5f;
            paraquedasAtivado = true;

            playerAudio.PlayOneShot(paraquedasSom, 1.0f);
        }
    }
}