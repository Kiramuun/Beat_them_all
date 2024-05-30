using System.Collections;
using UnityEngine;

/// <summary>
/// Composant StaminaManager pour g�rer l'endurance d'un joueur ou d'un personnage
/// </summary>
[AddComponentMenu("Dirk Dynamite/StaminaManager")]
[DisallowMultipleComponent]
public class StaminaManager : MonoBehaviour
{
    #region Public Variables
    [Header("Stamina Settings")]
    [Tooltip("Valeur maximale de l'endurance")]
    [Range(0f, 100f)]
    public int max = 100;
    [Tooltip("Valeur actuelle de l'endurance")]
    public int current;
    [Tooltip("Temps de recharge de l'endurance (en secondes)")]
    [Range(0f, 10f)]
    public float cooldown = 2f;
    [Tooltip("Taux de r�g�n�ration de l'endurance (par seconde)")]
    [Range(0f, 20f)]
    public float regenRate = 0.1f;
    [Tooltip("Palier du statut fatigue")]
    [Range(0f, 1f)]
    public float tiredTreshold = 0.3f;

    [Space(10)]

    [Header("Tired object")]
    [Tooltip("GameObject � activer")]
    public GameObject tiredAnimation;

    [Space(10)]

    [Header("Debug")]
    [Tooltip("Active le mode debug pour afficher les logs dans la console")]
    public bool debugMode = false;
    #endregion

    #region Private Variables
    [SerializeField]
    private float _chrono = 0f; 
    private bool _coolDownStart = false;
    private Coroutine regenCoroutine = null;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initialisation de l'endurance au d�marrage du jeu au niveau de sa valeur maximale
    /// </summary>
    void Start()
    {
        current = max;
        LogDebug( "La stamina a �t� initialis�e � " + current );
    }

    void Update()
    {
        if ( current < max )
        {
            if ( _coolDownStart )
            {
              _chrono += Time.deltaTime;
            }
            if ( _chrono >= cooldown )
            {
                _coolDownStart = false;
                LogDebug( "Temps de recharge �coul� - lancement de la r�g�n�ration de la stamina" );
                StartRegen();
                _chrono = 0f;
            }

        
            if (current <= tiredTreshold * max)
            {
                tiredAnimation.SetActive(true);
            }
            else
            {
                tiredAnimation.SetActive(false);
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Fonction pour consommer de l'endurance (ex: sprint, attaque sp�ciale, etc.)
    /// </summary>
    /// <param name="cost"> Co�t en endurance � consommer </param>
    /// <remarks> Retourne true si l'endurance est suffisante pour la consommation, false sinon et lance la fonction Not_Enough </remarks>
    public bool Consume( int cost )
    {
        if ( current >= cost )
        {
            current -= cost;
            _coolDownStart = true;
            LogDebug( "Stamina consomm�e " + cost + ". Stamina actuelle : " + current );
            StopRegen();
            return true;
        }
        else
        {
            Not_Enough();
            return false;
        }
    }

    /// <summary>
    /// Fonction pour r�g�n�rer de l'endurance (ex: repos, potion, etc.)
    /// </summary>
    /// <param name="stamina"> Quantit� d'endurance � r�g�n�rer </param>
    public bool Regen( int stamina)
    {
        if ( current < max )
        {
            if ( current + stamina > max )
            {
                current = max;
                LogDebug( "Stamina au maximum" );
                return true;
            }
            else
            {
                current += stamina;
                LogDebug( "R�g�n�ration de " + stamina + " points de stamina" );
                return true;
            }
        }
        else
        {
            LogDebug("Stamina au maximum");
            return false;
        }
    }

    /// <summary>
    /// Fonction de test pour consommer de l'endurance via l'inspecteur Unity
    /// </summary>
    [ContextMenu( "Consume Stamina Test (20)" )]
    public bool TestConsume()
    {
        return Consume( 20 );
    }

    /// <summary>
    /// Fonction de test pour consommer de l'endurance via l'inspecteur Unity
    /// </summary>
    [ContextMenu("Consume Stamina Test (20)")]
    public bool TestRegen()
    {
        return Regen(20);
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Fonction de r�g�n�ration de l'endurance via une coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator RegenCoroutine()
    {
        while (current < max)
        {
            current += 1;
            if (current > max)
            {
                current = max;
            }
            LogDebug("R�g�n�ration de la stamina. Stamina actuelle: " + current);
            yield return new WaitForSeconds(regenRate);
        }
    }

    /// <summary>
    /// D�marrer la r�g�n�ration
    /// </summary>
    private void StartRegen()
    {
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenCoroutine());
        }
    }

    /// <summary>
    /// Arr�ter la r�g�n�ration
    /// </summary>
    private void StopRegen()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
    }

    /// <summary>
    /// Fonction appel�e si l'endurance n'est pas suffisante pour la consommation (pour l'instant un simple log)
    /// </summary>
    private void Not_Enough()
    {
        LogDebug( "Pas assez de stamina pour la consommation" );
    }

    /// <summary>
    /// Fonction de log en mode debug pour afficher des messages dans la console Unity
    /// </summary>
    /// <param name="message"> R�ceptionne le message � afficher </param>
    /// <remarks> Cette fonction n'est appel�e que si le mode debug est activ� </remarks>
    private void LogDebug( string message )
    {
        if ( debugMode )
        {
            Debug.Log( message );
        }
    }
    #endregion
}
