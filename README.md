# ScalaTestTask

### Описание проделанной работы:
- Создан сервер WEB API отправляющий статистику о ценах на нефть за различные промежутки времени
 - Исходные данные для сервера берутся из URI указанного в задании
 - Проивзодится загрузка CSV файла с указанного URI c дальнейшим парсингом данных
 - Данные добавляются в mock хранилище, взаимодействие с которым производится через AppDBContext
 - Реализованы все необходимые по заданию методы

 ### Описание дополнительного функционала
  - Парсинг исходного CSV файла с учетом особенностей кодировки windows-1251
 - Добавлено логирование при каждом запросе
 - Возможность выбора другого источника данных (меняется в файле конфигурации appsettings.json)
 - Валидация входных данных 
 - Стандартный статистический анализ данных

## Описание REST архитектуры
`GET /statistics/prices` - Вернет полный перечень данных о ценах prices (в формате Json), где `prices` - Json файл с массивом JSON обектов с ценой и датой начала и конца промежутка
`GET //statistics/prices/{date}` - Вернет цену на нефть в указанню дату, где `date` - дата в формате dd/MM/yyyy  

`GET /statistics/prices/minmax/{start_date}/{end_date}` - Вернет минимальную и максимальную цены (в формате Json) за промежуток времени,
где
`start_date` - дата в формате dd/MM/yyyy начала промежутка времени,
`end_date` - дата в формате dd/MM/yyyy конца промежутка времени 

`GET /statistics/prices/average/{start_date}/{end_date}` - Вернет среднюю цену за промежуток времени,
где
`start_date` - дата в формате dd/MM/yyyy начала промежутка времени,
`end_date` - дата в формате dd/MM/yyyy конца промежутка времени 

`GET /statistics/prices/math_expectation/{start_date}/{end_date}` - Вернет математическое ожидание M цены за промежуток времени,
где
`start_date` - дата в формате dd/MM/yyyy начала промежутка времени,
`end_date` - дата в формате dd/MM/yyyy конца промежутка времени 

`GET /statistics/prices/variance/{start_date}/{end_date}` - Вернет дисперсию D цены за промежуток времени,
где
`start_date` - дата в формате dd/MM/yyyy начала промежутка времени,
`end_date` - дата в формате dd/MM/yyyy конца промежутка времени 

`GET /statistics/prices/standard_deviation/{start_date}/{end_date}` - Вернет стандартное отклонение σ цены за промежуток времени,
где
`start_date` - дата в формате dd/MM/yyyy начала промежутка времени,
`end_date` - дата в формате dd/MM/yyyy конца промежутка времени 

`GET /statistics/prices/linear_regression/{start_date}/{end_date}` - Вернет линейную регрессию цены (в формате JSON) за промежуток времени,
где
`start_date` - дата в формате dd/MM/yyyy начала промежутка времени,
`end_date` - дата в формате dd/MM/yyyy конца промежутка времени 

## Описание входных параметров
<table>
  <tr>
    <td>Имя</td><td>Тип</td><td>Пример</td><td>Описание</td>
  </tr>
  <tr>
    <td>date</td><td>DateTime</td><td>01/06/2015</td><td>Дата, требуемый формат: dd/MM/yyyy</td>
  </tr>
    <tr>
    <td>start_date</td><td>DateTime</td><td>01/06/2015</td><td>Дата, требуемый формат: dd/MM/yyyy, требуемое условие: <code>start_date < end_date</code></td>
  </tr>
    <tr>
    <td>end_date</td><td>DateTime</td><td>01/09/2015</td><td>Дата, требуемый формат: dd/MM/yy, требуемое условие: <code>start_date < end_date</code></td>
  </tr>
</table>

## Описание выходных параметров
<table>
  <tr>
    <td>Имя</td><td>Тип</td><td>Пример</td><td>Описание</td>
  </tr>
  <tr>
    <td>M</td><td>double</td><td>280.2</td><td>Математическое ожидание</td>
  </tr>
  <tr>
    <td>D</td><td>double</td><td>3180.96</td><td>Дисперсия</td>
  </tr>
  <tr>
    <td>σ</td><td>double</td><td>280.2</td><td>Стандартное отклонение</td>
  </tr>
  <tr>
    <td>prices</td><td>Json</td><td><code>[  {"start_date":2013-10-15T00:00:00, "end_date":2013-11-14T00:00:00,"price":384.2},
    {"start_date": "2013-11-15T00:00:00","end_date": "2013-12-14T00:00:00","price": 802.2}]</code></td><td>перечень данных о ценах</td>
  </tr>
  <tr>
    <td>minmax</td><td>json</td><td><code>{"min": 250, "max": 384.2}</code></td><td>Минимум и максимум цены на промежутке вермени</td>
  </tr>
  <tr>
    <td>linear_regression</td><td>json</td><td><code>{"expression":"y(x)=2*x+15.4","k"=2,"b"=10}}</code></td><td>Линейная регрессия цены описываемая функцией y=k*x+b</td>
  </tr>
</table>