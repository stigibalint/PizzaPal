# PizzaPál

![Group](https://github.com/user-attachments/assets/ffe6a857-ada6-4df6-aae1-69407ef27d77)

A PizzaPál projekt tagjai Stiegelmayer Bálint, Orosz Zsombor Olivér és Kecskés Dominik Bálint

**A folyamatokat így osztottuk meg**:
- Stiegelmayer Bálint: Frontend, Backend
- Orosz Zsombor Olivér: Adatbázis, Backend
- Kecskés Dominik Bálint: Trello kezelés, dokumentáció

---------------------------------------------------------------------------------------------------

A feladat egy egyszerű étterem alkalmazás, ahol két szerepkör van nyilvántartva:
- felhasználó
- admin

# Felhasználói felület
Bárki, aki nem admin fokozatban van vagy nincs bejelentkezve ezzel a felülettel találja szembe magát.
![image](https://github.com/user-attachments/assets/517d17d0-a21a-4798-99a7-0225c9f921e8)

Ha valaki be van jelentkezve, akkor a 'Bejelentkezve' szöveg átíródik a saját felhasználónevére, amire ha rákattint, akkor megnyitja a módosítási menüt.
![image](https://github.com/user-attachments/assets/aa86b886-5f16-4901-9e66-0dcfa30a6b08)

Másképpen pedig így jelenik meg
![{3E894F99-E813-4D1C-8354-3FAA0BDFA2CC}](https://github.com/user-attachments/assets/ec00c053-3ba6-4ed1-a240-2ad6c8af35f9)

# Bejelentkezés
A nem bejelentkezett felhasználók nem tudják a rendelést leadni addig amíg nincsenek bejelentkezve, amit a 'Bejelentkezés' gombra kattintva tehetnek meg.
![{76A04C2C-5B36-4472-A3CF-BEAB2CDB9B4F}](https://github.com/user-attachments/assets/722b5593-a733-4cf4-84be-19e6f643a076)
![{8A2C28EE-8E76-46EB-8F78-0E239AE0B66E}](https://github.com/user-attachments/assets/0331d6c0-0c5d-42ba-bb31-985c082f205e)

# Rendelés
Rendelés során a felhasználó hozzáadhat vagy eltávolíthat egy pizzát a kosárból.
![{A09C66C3-5117-40CD-AF20-7D84FB5DC32A}](https://github.com/user-attachments/assets/7e8c149c-390b-4066-b35c-6e63bb3b3eca)
![{DB1D2EE1-6281-4D7A-A33B-5EE385331FE5}](https://github.com/user-attachments/assets/a0ddfd8f-d39c-4153-b23b-0d4c88772135)
![{93C52595-3A9C-4F01-BCA0-C3A5CFB99261}](https://github.com/user-attachments/assets/c686bdef-aed8-446f-94a3-ad67b2c1181e)

Ha a kosár üres, akkor a felhasználó ezt nem fogja tudni megtenni, amit egy hibaüzenet dob.
![{F874EA04-D61C-4964-A640-FF054B2A7409}](https://github.com/user-attachments/assets/c718535e-8fc8-4730-8a7f-9d5f595aaf58)

Ha van legalább egy termék a kosárban, akkor már csak a szállítási címet kell megadni, és a fizetés gombra kattintva rendeli meg a pizzát.
![{1FC58F45-7534-48CD-8CF5-EC308B2F87FE}](https://github.com/user-attachments/assets/193d4c1a-23f8-4d36-a48e-5d56d1e73afa)
![{9F5CC3F4-08CC-43F9-91D4-B357127F16CB}](https://github.com/user-attachments/assets/9c281b0f-f11a-471a-83cc-42330d274627)

De ha nincs szállítási cím megadva, akkor a rendszer hibát jelez.
![{F2406951-0FCD-4033-BCDA-0867EA50FD23}](https://github.com/user-attachments/assets/516d5ae9-3092-4a9e-a332-c14f07d3d857)

---------------------------------------------------------------------------------------------------

# Admin felület
Az admin felületen lehet új pizzát felvenni vagy törölni, és a raktárt is kezelni.
![{365CB126-26EE-4F91-8BFA-92597A90AF40}](https://github.com/user-attachments/assets/0fa34b1c-99b2-4581-a89b-c5722a566629)
![{4026FB1B-AAC6-4A81-A13A-E3D56C83C87C}](https://github.com/user-attachments/assets/eaef429b-4d80-48e1-bb3a-bfd1b0f50daa)

# Pizza kezelés
Ha új pizzát szeretnénk hozzáadni, akkor beírjuk a nevét, az árát, kiválasztjuk az alapanyagokat és adunk hozzá egy képet.
![{C1BD1AAF-927B-4790-A5B2-439CA29952BD}](https://github.com/user-attachments/assets/25635034-f1a0-47d5-a543-d2382aac926a)
![{F4AB5963-4B0A-4E11-A64B-25C3EEB31424}](https://github.com/user-attachments/assets/9e9126a9-0940-4a66-882e-e516e03d2b08)
![{81548C0E-3907-4CDF-963C-4850F89079EF}](https://github.com/user-attachments/assets/6128e261-36c5-43bd-9152-63cdd7fb723b)

Ha bármi is hiányzik, akkor hibaüzenetet dob a rendszer.
![{26C8A0DB-A4C4-4E5D-A5D0-EBDCD3C836D2}](https://github.com/user-attachments/assets/c37c8c48-8d4f-403a-88d0-8d83ae7c6192)

Egy pizza törlésénél a menü végéhez térve a kuka ikonra kattintva tehetjük ezt meg.
![{5982E851-80B7-45A1-A450-13D73AC8F4A9}](https://github.com/user-attachments/assets/263d8510-308d-4386-8acd-9b607c323e4a)

Ha kiválasztottunk egy törölni kívánt pizzát, akkor a rendszer megkérdezi, hogy tényleg törölni szeretnénk a pizzát, amit az igen gombra kattintva tehetünk meg.
![{FBB6687A-941A-4C64-AC10-266443727F06}](https://github.com/user-attachments/assets/87699b12-156e-4a1c-8897-f11899df6c71)
![{3D70EE67-3B74-4AD1-91F8-AEB221065FE8}](https://github.com/user-attachments/assets/cd24f563-eec8-4314-a593-dd73276d25fe)

# Raktár kezelés
A raktár menüpontban az admin tudja változtatni az alapanyagokat.

Ha új alapanyagot akar hozzáadni az admin, akkor meg kell adni neki egy nevet és mennyiséget.
![{1E8AAA56-5FA9-4E70-A6BB-61608A38BC41}](https://github.com/user-attachments/assets/b692de7a-389a-426f-9e2e-2c89bd5c3978)

Csak akkor tud hozzáadni egy új alapanyagot, ha mind a név és a mennyiség megvan adva.
![{9E0A2EE1-1448-4639-9950-A906DA8F8AC6}](https://github.com/user-attachments/assets/ed48c3a8-33c2-44be-8934-198397baddc5)

Másképpen hibát dob a rendszer.
![{F4ABE71F-5131-45E9-8E9E-EFE3650DBD63}](https://github.com/user-attachments/assets/edaa7219-9907-46f0-bb76-d0862250903d)

Ha módosítani szeretnénk egy alapanyag nevét vagy mennyiségét, akkor azt az úgy teheti meg az admin, hogy belekattint a szövegbeviteli mezőbe, hogy átírja az értékét.
![{603FF1F3-3DDA-47CA-8D1D-4A562BDE1209}](https://github.com/user-attachments/assets/90c984ca-098b-4c22-be91-c6f20da4bfbc)

Ha az admin törölni szeretne egy alapanyagot, akkor azt úgy teheti meg, hogy rákattint a kuka ikonra, ami feldob egy olyan üzenetet, amivel rákérdez, hogy biztosan ki akarja-e törölni ezt az alapanyagot, ezáltal minden pizzát, ami tartalmazza ezt az alapanyagot, szintén törli.
![{2597AA39-7D0A-41AE-8940-A2876C1918DF}](https://github.com/user-attachments/assets/61113471-34a6-4c52-9289-c4f0fa960f1a)

Ha az igenre kattint, akkor végrehajta a törlést.
![{BF4AB4C7-5669-41BF-992C-96621DE5C853}](https://github.com/user-attachments/assets/96b5dc4d-724b-41ce-9ebd-5a5066e8857a)
