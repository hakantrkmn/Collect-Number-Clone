# Collect-Number-Clone

## Özellikler

### Puzzle Settings Scriptable Object

`script/scriptable` klasöründe bulunan `Puzzle Settings` scriptable objesi ile grid oluşturulabilir. Aşağıdaki parametreler ayarlanarak grid özelleştirilebilir:

- **numbers**: Sayıların hangi sayılar olacağını ve hangi renge sahip olacaklarını belirler.
- **width**: Grid'in genişliğini belirler.
- **height**: Grid'in yüksekliğini belirler.
- **spacing**: Grid hücreleri arasındaki boşluğu belirler.
- **matrix values**: Matrix'in belirli indekslerine özel değerler atanmasını sağlar.

Bu ayarları düzenledikten sonra `Create Grid` butonuna basarak grid oluşturabilirsiniz.

### Game Manager

Sahnede bulunan `GameManager` objesindeki `targets` array'ini düzenleyerek level hedeflerini belirleyebilirsiniz.

### Grid Ölçüleri

Grid ölçülerini düzenlemek için sahnede `Canvas -> InGameUI -> Grid` objesine gidip doğrudan ölçüleri ayarlayabilirsiniz. Kod, hücrelerin kare olacak ve ortada olacak şekilde oluşturulmasını sağlar.

## Kullanılan Paketler

- **DoTween**: Animasyonlar için kullanılmıştır.
- **Odin Inspector**: Gelişmiş editör özellikleri için kullanılmıştır.

## UI Düzenlemeleri

UI ile ilgili case dosyasında bir şey belirtilmediği için o kısıma zaman harcamadım. Case sonuçlanınca düzenleyeceğim.

---

### Kullanım Adımları

1. **Grid Ayarlarını Yapın**: `Puzzle Settings` scriptable objesindeki ayarları düzenleyin.
2. **Grid Oluşturun**: `Create Grid` butonuna basarak grid'i oluşturun.
3. **Level Hedeflerini Belirleyin**: `GameManager` objesindeki `targets` array'ini düzenleyin.
4. **Grid Ölçülerini Ayarlayın**: `Canvas -> InGameUI -> Grid` objesinde grid ölçülerini ayarlayın.

Bu adımları takip ederek kolayca grid oluşturabilir ve oyun hedeflerinizi belirleyebilirsiniz.

---

### Proje Gereksinimleri

- Unity 2020.3 veya üstü
- DoTween
- Odin Inspector
