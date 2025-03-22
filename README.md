# Система пропусков занятий
Система предназначена для контроля пропусков студентов.

Система позволяет студентам создавать заявку на пропуск. В этой заявке они могут описать причины, по которым отсутствовали, и прикрепить изображения документов (например, справки об освобождении от занятий).

Преподаватели могут посмотреть причины пропусков студентов и экспортировать файл с пропусками определенных студентов за указанный промежуток времени.

Работники деканата проверяют заявки студентов. Заявка может быть одобрена или отклонена.

Администратор системы назначает работников деканата и следит за корректной работой системы.


## Содержание
- [Технологии](#технологии)
- [Установка](#установка)
- [Документация](#документация)
- [Требования](#требования)
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

Создайте в директории class_absences_backend/WebAPI/ файл appsettings.json и вставьте данный код (При необходимости можно изменить настройки приложения):
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

Перейдите в папку решения:
```sh
cd class_absences_backend/
```

Выполните миграцию базы данных:
```sh
dotnet ef database update -p ./BusinessLogic/ -s ./WebAPI/ 
```

Забилдите проект:
```sh
dotnet build
```

Запустите проект:
```sh
dotnet run -p ./WebAPI/
```


### Требования
Для работы проекта необходим [PostgreSQL](https://www.postgresql.org/download/).


### Документация
После запуска приложения, его можно протестировать, а также посмотреть документацию через swagger ui.

Базовый путь, если appsettings.json не был изменен: [http:/localhost:5693/swagger/index.html](http:/localhost:5693/swagger/index.html)

## Команда проекта
- [Беловодченко Кирилл](https://github.com/Faddellin)
- [Сидоров Михаил](https://github.com/mikhail-belii)
