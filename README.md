#KAUNO TECHNOLOGIJOS UNIVERSITETAS
#INFORMATIKOS FAKULTETAS

#Saityno taikomųjų programų projektavimas (T120B165)
#Projektinio darbo ataskaita

#Atliko: 
#	IFF-1/7 gr. studentė
#	Gabija Skučaitė
#Priėmė:
#	Prof. Blažauskas Tomas 


1.	Projekto užduotis


Žodžių sudarymo žaidimo „Craft Of Word“
Sistema skirta žodžių sudarymo žaidimui „Craft Of Word“ žaisti internete. Žaidimas pritaikytas su kitais asmeninis varžytis turnyruose, peržiūrėti turnyrų istoriją. Taip pat žaidime už sudarytus žodžius gaunami taškai.
Sistema realizuojama naudojant REACT, .NET, o jai patalpinti Microsoft Azure platforma.
Objektai įgyvendinimui:
Vartotojas:
•	Unikalus žaidėjo ID;
•	El. Pašto adresas;
•	Slaptažodis;
•	Taškai.
Turnyras:
•	Turnyro ID;
•	Pavadinimas;
•	Pradžios ir pabaigos datos;
•	Laimėtojo ID.
Raundas:
•	ID;
•	Trukmės laikas.
Žodis:
•	Žodis;
•	Kuriam raundui priklauso žodis;
•	Vartotojo ID gavusio šį žodį;
•	Taškai už žodį.

Sistemoje galima:
•	Registruoti naują vartotoją;
•	Prisijungti prie sistemos;
•	Gauti visų vartotojų informaciją;
•	Atnaujinti vartotojo informaciją;
•	Ištrinti vartotoją;
•	Sukurti turnyrą;
•	Peržiūrėti turnyro informaciją;
•	Ištrinti turnyrą;
•	Pridėti raundą;
•	Koreguoti raundą;
•	Gauti raundo informaciją;
•	Pridėti žodį;
•	Gauti žodžio informaciją;
•	Pakeisti turnyro nugalėtoją;




4.	API specifikacija

1.	POST: RegisterUser sukuria vartotoją su pateikta informacija:
a.	Name: string:
b.	Email: string
c.	Password: string
d.	Points: 0
Responses:
201 – created
400 – bad request
409 – conflict

2.	GET: GetAllUsers gauna sąrašą visų vartotojų:
Responses:
200 – OK

3.	POST: UserLogin autentifikuoja vartotoją ir grąžina JWT token‘ą
a.	Email: string
b.	Password: string
Responses:
200 – OK
401 – Unauthorized

4.	GET: GetUserInformation gaunama informacija apie specifinį vartotoją
a.	ID
Responses:
200 – OK
404 – Not Found

5.	PUT: UpdateUserInformation atnaujinama specifinio vartotojo informacija
a.	Name: string
b.	Points: 0
Responses:
204 – No Content
401 – Unauthorized
403 – Forbidden
404 – Not Found

6.	DELETE: DeleteUser ištrinamas vartotojas pagal id
a.	ID
Responses:
204 – No Content
401 – Unauthorized
403 – Forbidden
404 – Not Found

7.	POST: CreateTournament sukuriamas naujas turnyras
a.	Name: string
b.	Startdate: date
c.	Enddate: date
d.	winnerID: 0
Responses:
201 – created
400 – bad request

8.	GET: ViewTournamentDetails gaunama informacija apie specifinį turnyrą
a.	ID
Responses:
200 – OK
404 – Not Found

9.	DELETE: DeleteTournament, ištrinamas turnyras pagal ID
a.	ID
Responses:
204 – No Content
404 – Not Found

10.	GET: GetTournamentRoundWords
a.	ID
b.	RoundID
Responses:
200 – OK
404 – Not Found

11.	PUT: UpdateTournamentWinner pakeičiamas turnyro nugalėtojas
a.	ID (turnyro)
b.	winnerID
Responses:
204 – No Content
404 – Not Found

12.	POST: AddParticipantToTournament pridedamas vartotojas prie turnyro
a.	ID (turnyro)
b.	userID
Responses:
204 – No Content
404 – Not Found

13.	POST: AddRound pridedamas naujas roundas prie turnyro
a.	ID (turnyro)
b.	Starttime: date
c.	Length: time
Responses:
201 – Created
404 – Not Found

14.	PUT: EditRoundData tvarkomos roundo detalės
a.	RoundID
b.	TournamentID
c.	Starttime: date
d.	Length: time
Responses:
204 – No Content
404 – Not Found

15.	GET: GetRoundData gaunama informacija apie specifinį roundą
a.	ID
Responses:
200 – OK
404 – Not Found

16.	POST: AddWord:
a.	Content: string
b.	RoundID
c.	UserID
d.	Points: 0
Responses:
201 – Created
404 – Not Found

17.	GET: GetWordData, gaunama informacija apie specifinį žodį
a.	ID
Responses:
200 – OK
404 – Not Found
