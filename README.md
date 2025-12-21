# MOSZE - 2D Roguelike Játék

##  Játék Leírása

2D roguelike játék, ahol a játékosnak egy űrállomáson kell legyőznie a meghibásodott robot biztonsági szolgálatot, túszul ejtett tudósokat kell megmentenie és végül legyőznie a főellenséget. A játék procedurálisan generált pályákat használ, így minden kör más.

##  Főbb Funkciók

###  Procedurális Pályagenerálás
- **Véletlenszerű térképek**: Minden játék egyedi, seed alapú pályagenerálással
- **Szobák és folyosók**: Dinamikusan generált szobák, amelyeket folyosók kötnek össze
- **JSON Export/Import**: Pályák exportálása és importálása JSON formátumban
- **Seed rendszer**: Ugyanaz a seed ugyanazt a pályát generálja

###  Harcrendszer
- **Körökre osztott harc**: Játékos és ellenségek váltakozva lépnek
- **Különböző ellenségtípusok**
- **AI viselkedés**: Ellenségek különböző taktikákat használnak (agresszív, védekező, óvatos, véletlenszerű)
- **Támadás és védelem**: Pontos sebzés számítás accuracy és defense értékekkel

###  Játékmenet
- **Túsok megmentése**: Minden túsz megmentése szükséges a győzelemhez
- **Boss harc**: Miután minden túsz meg van mentve, a boss megjelenik
- **Tárgyak gyűjtése**: Különböző típusú tárgyak (heal, stat boost, treasure)
- **XP és szintlépés**: Ellenségek legyőzése után XP-t kapsz, szintlépéskor stat boostokat

###  Mentés és Betöltés
- **Játék mentése**: Bármikor mentheted a játékodat
- **Highscore rendszer**: Legjobb pontszámaid mentése
- **Pálya exportálás**: Pályáidat exportálhatod és újra játszhatod

###  Audio Rendszer
- **Háttérzene**: Folyamatos háttérzene
- **Hangerőszabályozás**: Master, Music és SFX hangerő külön-külön beállítható

##  Célok

1. **Túszok megmentése**: Minden túszt meg kell mentened a pályán
2. **Boss legyőzése**: Miután minden túsz meg van mentve, a boss megjelenik
3. **Túlélés**: Éld túl az ellenségek támadásait
4. **Pontszám**: Minél több pontot gyűjts, hogy felkerülj a highscore listára

##  Irányítás

### Játék közben:
- **WASD** vagy **Nyilak**: Mozgás
- **ESC**: Pause menü megnyitása/bezárása

### Harc közben:
- **Mozgás gomb**: Irányválasztás (WASD vagy nyilak)
- **Támadás gomb**: Ellenség kiválasztása és támadás
- **Tárgy gomb**: Tárgy használata (pl. heal)
- **Kör vége gomb**: Kör befejezése

##  Új Játék Indítása

A főmenüben három lehetőség közül választhatsz:

1. **Véletlenszerű játék**: Teljesen random seed-del generált pálya
2. **Seed alapú játék**: Konkrét seed használata pálya generáláshoz
3. **JSON pálya betöltése**: Előzőleg exportált pálya betöltése


##  Projekt Struktúra

```
Assets/
├── Scripts/
│   ├── Core/          # Fő játékosztályok
│   ├── Combat/        # Harcrendszer
│   ├── Map/           # Pályagenerálás
│   ├── Systems/       # Rendszerek
│   ├── UI/            # Felhasználói felület
│   ├── Data/          # Adatstruktúrák
│   ├── AI/            # Viselkedés sémák
│   └── Utils/         # Segédfüggvények
└── Sprites/           # Sprite fájlok
```

## Jövőbeli Fejlesztések

- **Hangeffektek**: Lépés, támadás, sebzés, tárgygyűjtés, szintlépés, harc kezdés/vége, gomb kattintás
- **UI Update**: Beszédesebb UI, több visszajelzés a felhasználóknak
- **Kidolgozottabb harc**: Több fajta támadás, speed stat használata, több féle tárgy, célpont választás, menekülés
- **Animációk**: Statikus sprite-ok helyett valódi animációk.

## Fejlesztők: NextGen Devs

- Németh Rubin Doroti
- Vadász Bálint
- Rózsa Ádám
- Kalmár Patrik

**Jó játékot!**

