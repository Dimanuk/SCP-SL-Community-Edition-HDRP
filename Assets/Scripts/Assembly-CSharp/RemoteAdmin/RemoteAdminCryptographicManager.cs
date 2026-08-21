using System;
using GameCore;
using Mirror;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using UnityEngine;
using Console = GameCore.Console;

namespace RemoteAdmin
{
    public class RemoteAdminCryptographicManager : NetworkBehaviour
    {
        private AsymmetricCipherKeyPair _ecdhKeys;
        private ECDHBasicAgreement _exchange;
        private byte[] _ecdhPublicKeySignature;

        internal bool ExchangeRequested;
        internal byte[] EncryptionKey;

        private void Init()
        {
            _ecdhKeys = Cryptography.ECDH.GenerateKeys(384);
            _exchange = Cryptography.ECDH.Init(_ecdhKeys);
            ExchangeRequested = true;
        }

        [Server]
        public void StartExchange()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RemoteAdmin.RemoteAdminCryptographicManager::StartExchange()' called when server was not active");
                return;
            }

            if (_exchange == null || _ecdhKeys == null)
            {
                Init();
            }

            string publicKeyString = Cryptography.ECDSA.KeyToString(_ecdhKeys.Public);
            TargetDiffieHellmanExchange(connectionToClient, publicKeyString);
        }

        [TargetRpc]
        private void TargetDiffieHellmanExchange(NetworkConnection conn, string publicKey)
        {
            if (EncryptionKey != null)
            {
                Console.AddLog("Rejected duplicated Elliptic-curve Diffie–Hellman (ECDH) parameters from server.", Color.magenta);
                return;
            }

            if (_exchange == null || _ecdhKeys == null)
            {
                Init();
            }

            if (_ecdhPublicKeySignature == null)
            {
                string myPublicKeyString = Cryptography.ECDSA.KeyToString(_ecdhKeys.Public);
                AsymmetricKeyParameter authKey = CentralAuthManager.SessionKeys?.Private;
                if (authKey != null)
                {
                    _ecdhPublicKeySignature = Cryptography.ECDSA.SignBytes(myPublicKeyString, authKey);
                }
            }

            AsymmetricKeyParameter serverPublicKey = Cryptography.ECDSA.PublicKeyFromString(publicKey);
            EncryptionKey = Cryptography.ECDH.DeriveKey(_exchange, serverPublicKey);

            string myKeyString = Cryptography.ECDSA.KeyToString(_ecdhKeys.Public);
            CmdDiffieHellmanExchange(myKeyString, _ecdhPublicKeySignature);

            Console.AddLog("Completed ECDHE exchange with server.", Color.green);
        }

        // Safety net: if a client's TargetDiffieHellmanExchange was lost (e.g. sent before the
        // client identity finished spawning), it would be stuck with a null EncryptionKey forever
        // and every RA command would fail with "ECDHE exchange not performed". Let the client ask
        // the server to re-run the exchange; StartExchange re-validates authority on its own.
        [Command]
        public void CmdRequestEcdheExchange()
        {
            ReferenceHub hub = ReferenceHub.GetHub(gameObject);
            if (hub == null || !hub.serverRoles.RemoteAdmin)
                return;

            if (EncryptionKey != null)
                return;

            StartExchange();
        }

        [Command]
        private void CmdDiffieHellmanExchange(string publicKey, byte[] signature)
        {
            if (EncryptionKey != null || _exchange == null || _ecdhKeys == null)
                return;

            ReferenceHub hub = ReferenceHub.GetHub(gameObject);
            AsymmetricKeyParameter serverPublicKey = hub.serverRoles.PublicKey;
            string authToken = hub.characterClassManager.AuthToken;

            if (CharacterClassManager.OnlineMode)
            {
                if (publicKey == null || authToken == null)
                {
                    hub.gameConsoleTransmission.SendToClient(
                        "Please complete authentication before requesting ECDHE exchange.",
                        "magenta");
                    return;
                }

                if (serverPublicKey != null && !Cryptography.ECDSA.VerifyBytes(publicKey, signature, serverPublicKey))
                {
                    hub.gameConsoleTransmission.SendToClient(
                        "Exchange parameters signature is invalid!",
                        "magenta");
                    return;
                }
            }

            AsymmetricKeyParameter clientPublicKey = Cryptography.ECDSA.PublicKeyFromString(publicKey);
            EncryptionKey = Cryptography.ECDH.DeriveKey(_exchange, clientPublicKey);
        }
    }
}