### News Aggragator

Web programma, kas ik pa laikam sauc API - https://github.com/cyberboysumanjay/Inshorts-News-API - un attēlo saņemtos datus.


### Palaišana

Programma veidota un ir pārbaudīti palaižama ar Docker for Windows. Lai to izdarītu:

- Docker for Windows jāpārslēdz uz Linux konteineriem
- Jāizveido image: `docker build . -t helmesproject` 
- No izveidotā image jāpalaiž konteiners: `docker run -d -p 8090:80 helmesproject`

Pēc palaišanas, programmu iespējams apskatīt pārlūkā, navigējot uz `localhost:8090/`

