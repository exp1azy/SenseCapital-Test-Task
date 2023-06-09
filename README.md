# SenseCapitalBackTask (Тестовое задание для SENSE CAPITAL)
API написано на платформе ASP.NET, в качестве базы данных выступает PostgreSQL, приложение завернуто в docker-compose.
## Описание
Приложение представляет из себя веб-API для крестиков-ноликов и содержит:
1) Регистрацию и авторизацию/аутентификацию (с помощью jwt-токенов) игроков + хэширование паролей
2) Возможность "бросить вызов" другому игроку, создав при этом новую игру
3) Возможность получить список игр, в которых участвует текущий игрок (mode = "in" - список игр, где ожидается ход текущего игрока, mode = "out" - список игр, где ожидается ход соперника)
4) Совершение хода в игре в соответствии с правилами крестиков-ноликов (при этом происходят соответствующие проверки на ничью, победу и т.д.)
5) Получение игры по её идентификатору
## Как протестировать
Для начала у вас должен быть установлен Docker desktop. Запускать проект нужно в VS22 через docker-compose
1) Зарегистрируйте игрока
2) Авторизуйтесь
3) Скопируйте токен и авторизуйтесь в swagger (при авторизации предварительно напишите Bearer и через пробел вставьте токен)
4) Далее можете зарегистрировать соперника и тестировать остальные запросы 
