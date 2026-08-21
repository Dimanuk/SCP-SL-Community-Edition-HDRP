# Local Central Server (SCP:SL 12.0.2 test auth)

A tiny stand-in for `api.scpslgame.com` so you can run the game in **online mode** on a private
network (LAN / **Radmin VPN**) without Northwood's servers, Steam tickets or the launcher.

It is **trust-based**: whatever UserId/Nickname a client asks for, it gets. Run it **only** on a
private network.

---

## What it does

It serves exactly the endpoints the game hits:

| Endpoint | Used by | Purpose |
|---|---|---|
| `GET /v5/publickey.php?major=12` | dedicated/host server | central public key signed by the master key |
| `GET /servers.php` | everyone | server-mirror list (returns `API`) |
| `GET /ip.php` | dedicated/host server | echoes the caller's IP (external-IP lookup at startup) |
| `POST /v5/authenticator.php` | dedicated/host server | server-list heartbeat/registration (served back via lobbylist) |
| `POST /lobbylist.php` | client (server browser) | **signed server list** of servers registered via authenticator.php |
| `POST /v5/contactaddress.php` | dedicated server | acknowledged (no-op) |
| `POST /v5/steam/authenticate.php` | client | issues api-token + nonce + **preauth token** |
| `POST /v5/requestsignature.php` | client | issues the full **authentication token** |
| `POST /v5/renew.php` | client | session refresh (same shape as authenticate) |
| `POST /centralcommands/*.php` | dedicated server | acknowledged (no-op) |

The server list is signed as `ECDSA(payload + "##" + timestamp)` with the **central** key —
exactly what `NewServerBrowser`/`ServerListManager` verify against `ServerConsole.PublicKey`.
Game servers appear in the list while they keep heartbeating (TTL 120 s).

Tokens are signed with the **central** key; the central key is signed with the **master** key.
The game verifies the central key against the master public key compiled into
`CentralServerKeyCache.LocalMasterPublicKey`.

## Crypto compatibility

The project references the game's own `Assets/Plugins/BouncyCastle.Crypto.dll`, so PEM keys and
`SHA-256withECDSA` signatures are byte-compatible. No NuGet / internet needed.

---

## 1. Build & run the server

```sh
cd Tools/LocalCentralServer
dotnet build
# verify crypto end-to-end (optional):
dotnet run -- --selftest

# run it (needs admin OR a urlacl for the "+" binding — see below):
dotnet run -- --prefix http://+:5000/
```

On first run it generates `keys/` and prints the **master public key** as a ready-to-paste C#
literal. It is already pasted into the game (`CentralServerKeyCache.LocalMasterPublicKey`); if you
regenerate the keys (delete `keys/`), re-paste the new value from `keys/MasterPublicKey.csharp.txt`.

### Binding / firewall
`http://+:5000/` and hostname bindings need either:
- run the terminal **as Administrator**, or
- grant a URL ACL once (admin cmd): `netsh http add urlacl url=http://+:5000/ user=Everyone`

Then allow port 5000 through Windows Firewall (or `netsh advfirewall firewall add rule
name="LCS" dir=in action=allow protocol=TCP localport=5000`). Bind to the **Radmin VPN IP** so
friends can reach it. `--prefix http://localhost:5000/` works for single-machine testing without admin.

---

## 2. Point the game at it (every machine: host + clients)

1. Copy `localcentral.example.txt` → `localcentral.txt` into **`%AppData%\SCP Secret Laboratory\`**
   (same folder for a built game and the Unity Editor).
2. Set `url=` to the central server's Radmin IP, e.g. `http://26.119.65.236:5000/`.

The presence of `localcentral.txt` flips the game into local mode automatically.

### Identity (automatic)
The player's **SteamID64 and Steam name are taken automatically** from the running Steam client (no
auth ticket, just a local read). If Steam isn't running, a stable per-machine id is generated and the
Windows username is used. `userid=`/`nickname=` in `localcentral.txt` are optional overrides — only
needed to pin an identity or to run two clients on the **same PC/Steam account** (they'd otherwise
clash; the listen-server host is exempt since it stays `ID_Host`).

### Testing in the Unity Editor
Because the app folder is `%AppData%\SCP Secret Laboratory\`, the editor reads the same
`localcentral.txt`. Just drop the file there and press Play. With ParrelSync (host in the main editor,
client in the clone): the host stays `ID_Host` and the clone authenticates as your Steam account —
no collision.

## 3. Turn on online mode on the server

In `config_gameplay.txt` set `online_mode: true`. (Offline mode ignores all of this and needs no
central server.)

---

## Flow recap

```
client                          local central server                 dedicated/host server
  | authenticate.php  --------------->  issue preauth+api token  |
  | <--- id/nonce/preauth/api ---------                          |
  | requestsignature.php ------------>  issue signed auth token  |
  | <--- auth(base64) -----------------                          |
  | --- connect (preauth token in GetConnectData) ----------------->  verify w/ central pubkey
  | --- CmdSendToken(auth token) ---------------------------------->  verify -> assign UserId
```

## Files

- `Program.cs` — HTTP server + routing + selftest
- `KeyStore.cs` — generate/load/persist master & central keys
- `Tokens.cs` — preauth + auth token builders (formats dictated by the game's verifiers)
- `Crypto.cs` — ECDSA/PEM mirror of the game's `Cryptography.ECDSA`
