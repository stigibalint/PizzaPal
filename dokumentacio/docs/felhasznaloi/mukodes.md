---
icon: info
---
# Program működése

## Szerepkörök
A programban egy felhasználó 4 szerepkör egyikébe tartozhat:

- **Vendég**: Az alapértelmezett, a bejelentkezés nélküli felhasználó. Megtekinthet néhány adatot, de nem változtathat semmin.
- **Fogadó**: Regisztráció után egy felhasználó fogadóként kerül létrehozásra. Egy fogadó megtekintheti a játékok listáját, 
fogadást adhat le, valamint láthatja és törölheti a saját fogadásait.
- **Szervező**: Egy adminisztrátor szervezővé tehet egy regisztrált felhasználót. Egy szervező megtekintheti a játékok és a 
beérkezett fogadások listáját, új játékot hozhat létre, törölhet játékot bizonyos esetben, valamint rögzítheti a saját játékainak 
eredményeit.
- **Adminisztrátor**: Egy adminisztrátor megtekintheti a program regisztrált felhasználóit, és módosíthatja az adataikat. 
Ezen kívül módosíthatja a program email küldési beállításait.

A szerepkörök által végezhető műveletekről további információ található a felhasználói dokumentáció egyes oldalain.

## Fogadás
A játékok működésének megértéséhez vegyünk először egy konkrét játékot: „Lajos és Bettina programjának futása”. 
Ez a játék két *alanyt* tartalmaz: Lajost és Bettinát. Melléjük különböző *eseményeket* párosíthatunk (pl. programfutásának 
sebessége, programjának kimenete). Ebben a játékban a lehetséges alany-esemény párosok:

- Lajos programfutásának sebessége
- Lajos programjának kimenete
- Bettina programfutásának sebessége
- Bettina programjának kimenete

A fogadók egy adott alany-esemény párosra fogadhatnak. Megadhatnak egy *értéket*, ami szerintük az esemény eredménye lesz, 
és tetszőleges nagyságú tétet tehetnek fel. Egy fogadó egy játékon belül több fogadást is leadhat, de egy alany-esemény 
párosra csak egyet.

Amikor egy szervező lezár egy játékot, meg kell adnia minden alany-esemény párosnak az értéket. Ez alapján a program megállapítja 
a nyerteseket, és jóváírja a nyert pontokat.


## Pontok
Minden fogadó **100** ponttal kezd. Amikor lead egy fogadást, a feltett tét levonásra kerül. Miután egy játék lezárult, 
a program minden alany-esemény pároshoz megállapít egy szorzót, ami alapján a nyertes fogadók megkapják a nyert pontjaikat. 
Ha egy fogadás nem nyer, a feltett pontok végleg elvesznek. A program nagyobb szorzókkal jutalmazza a merészebb, egyedibb fogadásokat.