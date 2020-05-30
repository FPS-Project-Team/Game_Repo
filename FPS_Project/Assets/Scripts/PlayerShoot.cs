using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour{

    private const string PLAYER_TAG = "Player";
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    
    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    void Start (){
        if(cam == null){
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false; 
        }
        weaponManager = GetComponent<WeaponManager>();
    }


    void Update(){
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(currentWeapon.fireRate <= 0f){
            if(Input.GetButtonDown("Fire1")){
                Shoot();
            }
        }
        else{
            if(Input.GetButtonDown("Fire1")){
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1")){
                CancelInvoke("Shoot");
            } 
        }
    }

    [Command]
    void CmdOnShoot(){
        RpcDoShootEffect();
    }

    [ClientRpc]
    void RpcDoShootEffect(){
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }


    [Command]
    void CmdOnHIt(Vector3 _pos, Vector3 _normal){
        RpcDoHitEffect(_pos, _normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal){
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client]
    void Shoot(){
        if(!isLocalPlayer){
            return;
        }

        //We are shooting; call on shoot method
        CmdOnShoot();

        RaycastHit _hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)){
            if(_hit.collider.tag == PLAYER_TAG){
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }

            CmdOnHIt(_hit.point, _hit.normal);
        }
    }
    [Command]
    void CmdPlayerShot(string _playerID,  int _damage){
        Debug.Log(_playerID + " has been shot.");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
	}

}