![Centurion logo](https://raw.githubusercontent.com/unchase/contestmeter/master/Images/contestmeter_logo.png) 


# ContestMeter

Тестирующая система [ContestMeter](http://contestmeter.mmcs.sfedu.ru) предназначена для проведения соревнований по программированию (контестов) с автоматической проверкой решений участников по правилам [ACM](https://wikipedia.org/wiki/ACM_International_Collegiate_Programming_Contest) и [KYROV](https://school9.perm.ru/gate/articles/FAQ/#s_1_q_0).

*Все необходимые действия по загрузке и настройке дополнительного программного обеспечения будут произведены автоматически при установке.*

Для работы программы необходимо наличие:<br/>

- [.NET Framework 4.5](https://www.microsoft.com/ru-ru/download/details.aspx?id=30653) или выше
- [SQL Server 2012 Express LocalDB](https://www.microsoft.com/ru-RU/download/details.aspx?id=29062) или выше
- [IIS Server 7.5 Express](https://www.microsoft.com/ru-ru/download/details.aspx?id=1038) или выше.

<a href="https://github.com/unchase/ContestMeter/releases/latest" rel="nofollow"><img src="https://img.shields.io/github/downloads/unchase/ContestMeter/total.svg?maxAge=86400&&style=flat-square" alt="GitHub Releases (latest)"></a>
[![GitHub (Pre-)Release Date](https://img.shields.io/github/release-date-pre/unchase/contestmeter.svg?style=flat-square)](https://github.com/unchase/contestmeter/releases/latest)

## Supported OS
* Windows 7/8/8.1/10
* Windows Server 2008/2012/2016.

## Current status

Работа ведется над исправлением ошибок.

#### Version 1.0.0-pre

<table>
  <tr>
    <th>&nbsp;</th>
    <th>Windows</th>
    <th>Linux/Mac</th>
  </tr>
  <tr>
    <td>Runtime environment</td>
    <td>MS Windows MS 7/8/8.1/10<br/>MS Windows Server 2008/2012/2016<br/>.NET Framework 4.5+</td>
    <td>No official support</td>
  </tr>
  <tr>
    <td>Development</td>
    <td><a href="https://visualstudio.microsoft.com" width="49%">MS VS 2017 v15.5+</a>, C#7.2, ASP.NET MVC 5,<br/> EF Code first, WIX Toolset</td>
    <td>No official support</td>
  </tr>
  <tr>
    <td>Deployment</td>
    <td>IIS 6+</td>
    <td>No official support</td>
  </tr>  
  <tr>
    <td><strong>Latest Pre-Release (v1.0.0-pre)</strong></td>
    <td>GitHub: <a href="https://github.com/unchase/ContestMeter/releases"><img src="https://img.shields.io/github/downloads-pre/unchase/ContestMeter/latest/total.svg?maxAge=86400&&style=flat-square" alt="GitHub Releases (latest)"></a></td>
    <td>No official support</td>
  </tr>
</table>

## Features
*Основные преимущества перед существующими тестирующими системами:*

- Централизованная система проверки решений
- Автоматическая проверка решений с помощью checker'ов в изолированной среде
- Поддержка проведения нескольких типов олимпиад с возможностью дальнейшего расширения: ACM, KYROV (*contester.ru не позволяет менять способ оценки решений участников*)
- Открытый исходный код (*тестирующие системы Новосибирских и Уральских университетов, а также ТТИ ЮФУ, являются закрытыми*)
- Поддержка со стороны разработчиков (*у PCMC-2 отсутствует поддержка*)
- Интуитивно понятный интерфейс (*ejudge имеет громоздкий интерфейс, труднопереносима, труднонастраиваема и содержит множество ошибок*).

## Links
* Issue tracker: [![GitHub issues](https://img.shields.io/github/issues/unchase/contestmeter/shields.svg?style=flat-square)](https://github.com/unchase/contestmeter/issues) [![GitHub issues-closed](https://img.shields.io/github/issues-closed/unchase/contestmeter.svg?style=flat-square)](https://GitHub.com/unchase/contestmeter/issues?q=is%3Aissue+is%3Aclosed)
* Website: [![Website contestmeter.mmcs.sfedu.ru](https://img.shields.io/website-up-down-green-red/http/contestmeter.mmcs.sfedu.ru.svg?style=flat-square)](http://contestmeter.mmcs.sfedu.ru/)
* Wiki: <a href="https://github.com/unchase/contestmeter/wiki" rel="nofollow" target="_blank"><img src="https://img.shields.io/badge/Wiki-go-blue.svg?style=flat-square" alt="Github Wiki"></a>
