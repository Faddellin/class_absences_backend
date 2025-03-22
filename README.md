# Система пропусков занятий
Система предназначена для контроля пропусков студентов.

Система позволяет студентам создавать заявку на пропуск. В этой заявке они могут описать причины, по которым отсутствовали, и прикрепить изображения документов (например, справки об освобождении от занятий).

Преподаватели могут посмотреть причины пропусков студентов и экспортировать файл с пропусками определенных студентов за указанный промежуток времени.

Работники деканата проверяют заявки студентов. Заявка может быть одобрена или отклонена.

Администратор системы назначает работников деканата и следит за корректной работой системы.


## Содержание
- [Технологии](#технологии)
- [Установка](#установка)
- [Команда проекта](#команда-проекта)

## Технологии
- [ASP.NET Core](https://learn.microsoft.com/)
- [PostgreSQL](https://www.postgresql.org/)
- [Entity Framework Core](https://learn.microsoft.com/ru-ru/ef/core/)

## Установка

Клонируйте репозиторий проекта:
```sh
git clone https://github.com/Faddellin/class_absences_backend.git
```

Создайте в данной директории class_absences_backend/WebAPI/ файл appsettings.json и вставьте данный код:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "ConnectionStrings": {
    "AppDbContext": "Host=localhost;Database=AbsenceDB;Port=5432;Username=postgres;Password=123"
  },

  "Jwt": {
    "Issuer": "Class_Absences_server",
    "Audience": "Class_Absences_client",
    "Secret": "eriogoerj,94t9i,w0pevrr0pti,esgp,vgeop'tges",
    "ExpireInMinutes": 2
  },

  "RefreshToken": {
    "ExpireInDays": 2
  },

  "Kestrel": {
    "Endpoints": {
      "Endpoint": {
        "Url": "http://0.0.0.0:5693/"
      }
    }
  },

  "Scheduler": {
    "JwtRemoverIntervalInMinutes": 60
  }
}
```

Перейдите в папку библиотеки классов BusinessLogic проекта:
```sh
cd class_absences_backend/BusinessLogic/
```

Выполните миграцию базы данных:
```sh
dotnet ef database update
```

Перейдите в папку WebAPI и запустите проект:
```sh
cd ../WebAPI
dotnet run
```


### Требования
Для работы проекта необходим [PostgreSQL](https://www.postgresql.org/download/).


## Команда проекта
- [Беловодченко Кирилл](https://github.com/Faddellin)
- [Сидоров Михаил](https://github.com/mikhail-belii)
