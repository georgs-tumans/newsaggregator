### News Aggragator

Web programma, kas ik pa laikam sauc API - https://github.com/cyberboysumanjay/Inshorts-News-API - un attēlo saņemtos datus.
Veidota ar .NET 6 backend, Sqlight DB un Angular frontend. Datu attēlošanai tiek izmantota Angular Material tabula.

### Priekšnosacījumi
- Palaišanai ārpus Docker - datorā instalēts .NET 6 SDK un Angular CLI (priekš tā savukārt jābūt instalētai node.js)
- Palaišanai caur Docker - uzstādīts Docker Desktop for Windows (vismaz izstrādāts un pārbaudīts tika uz šī)

### Palaišana

- Docker for Windows jāpārslēdz uz Linux konteineriem
- Jāizveido image: `docker build . -t helmesproject`* 
- No izveidotā image jāpalaiž konteiners: `docker run -d -v logs:/my_logs -p 8090:80 helmesproject`*

*komandas izpilda mapē, kur atrodas Dockerfile.

Pēc palaišanas, programmu iespējams atvērt pārlūkā, navigējot uz `localhost:8090/`.

**NB:** Pie pirmās palaišanas Docker konteinerā novērots, ka pirmais API izsaukums var beigties ar Timeout, tā ka ļoti iespējams, ka pirmajā brīdī līdz atkārtotiem API izsaukumiem dati nebūs redzami.
Logu failus iespējams atrast zem volume 'logs', kura saturu ērti pārskatīt caur Docker Desktop lietotni.

### Konfigurācija
Projekts tiek konfigurēts ar šādiem iestatījumiem iekš Appsettings.json:
```
{
  "ConnectionStrings": {
    "local": "DataSource=helmes.db" //Sqlight DB faila nosaukums
  },
  "AllowedHosts": "*",
  "ApiUrl": "https://inshorts.deta.dev/news?category=all",  //URL uz API, ko sauc datu izguvei
  "RefreshInterval": 60, //API izsaukšanas (datu atjaunošanas) regularitātes interāls. kas norādāms sekundēs
  "LoggingPath": "../my_logs", //ceļs uz log failiem*
  "MinLogLevel": "Info", //Debug/Info/Warning/Error - var norādīt, kāds ir minimālais failā rakstāmais log līmenis
  "SaveDuplicates":  false //Tā kā regulāri izsaucot API tiek saņemti daudzi duplikāti, te var norādīt, vai tos atkārtoti saglabāt DB vai nē (un attiecīgi attēlot pārlūkā vai nē)
}
```
Konfigurācija jāveic pirms Docker image izveides.

*mapes nosaukumam šajā ceļā jāsakrīt ar Docker volume, ar kuru tiek palaists konteiners. Pēc noklusējuma iestatīts ceļš uz `my_logs` mapi un tāpat arī nosaukts izveidojamais volume palaišanas komandā.

