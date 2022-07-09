### News Aggragator

Web programma, kas ik pa laikam sauc API - https://github.com/cyberboysumanjay/Inshorts-News-API - un attēlo saņemtos datus.
Veidota ar .NET 6 backend un Angular frontend. Datu attēlošanai tiek izmantota Angular Material tabula.

### Palaišana

Programma veidota un ir pārbaudīti palaižama ar Docker for Windows. Lai to izdarītu:

- Jābūt uzstādītam Docker for Windows
- Docker for Windows jāpārslēdz uz Linux konteineriem
- Jāizveido image: `docker build . -t helmesproject`* 
- No izveidotā image jāpalaiž konteiners: `docker run -d -v logs:/my_logs -p 8090:80 helmesproject`*

*komandas izpilda mapē, kur atrodas Dockerfile.

Pēc palaišanas, programmu iespējams apskatīt pārlūkā, navigējot uz `localhost:8090/`.
Logu failus iespējams atrast zem volume 'logs', kura saturu ērti pārskatīt caur Docker Desktop lietotni.

### Visādi
