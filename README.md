# WeatherApp

Консольное приложение на C#: читает список городов из файла, получает текущую погоду с `https://wttr.in/{City}?format=j1` и выводит данные по городам и статистику по странам.

## Запуск

Нужен .NET SDK 10.

```bash
dotnet run --project src/WeatherApp
```

По умолчанию читается `src/WeatherApp/cities.txt`. Свой файл передаётся первым аргументом:

```bash
dotnet run --project src/WeatherApp -- /path/to/cities.txt
```

Тесты:

```bash
dotnet test
```

## Пример вывода

```
Moscow, Russia +11 °C
Khabarovsk, Russia +8 °C
Saint-Petersburg, Russia +11 °C
Vienna, Austria +15 °C
Izhevsk, Russia +9 °C
Perm, Russia +5 °C
NhaTrang, Vietnam +29 °C
Villach, Austria +11 °C

Russia — 5 cities, avg: +8.8 °C, min: +5 °C, max: +11 °C
Austria — 2 cities, avg: +13 °C, min: +11 °C, max: +15 °C
Vietnam — 1 city, avg: +29 °C, min: +29 °C, max: +29 °C
```

## Доступность wttr.in

Без VPN wttr.in не отвечает, либо отдаёт ответ не полностью.

Нужные поля (`temp_C` и страна) лежат в начале ответа, поэтому программу можно было сделать устойчивой к обрыву: читать ответ потоком и останавливаться, как только они получены. Я решил работать с целым ответом: разбор проще, а неполный JSON решил считать как невалидные данные.

## Внешние пакеты

Приложение не использует внешних пакетов: JSON разбирается встроенным `System.Text.Json`, повторы запросов и сборка зависимостей написаны вручную.

xUnit подключён только в проекте тестов `tests/WeatherApp.Tests` и в сборку приложения не входит.
