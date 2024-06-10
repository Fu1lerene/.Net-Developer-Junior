# Неделя 5: домашнее задание

## Перед тем как начать
- Как подготовить окружение [см. тут](./docs/01-prepare-environment.md)
- **САМОЕ ВАЖНОЕ** - полное описание базы данных, схему и описание поле можно найти [тут](./docs/02-db-description.md)
- Воркшоп и примеры запросов [см. тут](./docs/02-db-description.md)

## Основные требования
- решением каждого задания является ОДИН SQL-запрос
- не допускается менять схему или сами данные, если этого явно не указано в задании
- поля в выборках должны иметь псевдоним (alias) указанный в задании
- решение необходимо привести в блоке каждой задачи ВМЕСТО комментария "ЗДЕСЬ ДОЛЖНО БЫТЬ РЕШЕНИЕ" (прямо в текущем readme.md файле)
- метки времени должны быть приведены в формат _dd.MM.yyyy HH:mm:ss_ (время в БД и выборках в UTC)

## Прочие пожелания
- всем будет удобно, если вы будете придерживаться единого стиля форматирования SQL-команд, как в [этом примере](./docs/03-sql-guidelines.md)

## Задание 1: 100 заданий с самым долгим временем выполнения
Время, затраченное на выполнение задания - это период времени, прошедший с момента перехода задания в статус "В работе" и до перехода в статус "Выполнено".
Нужно вывести 100 заданий с самым долгим временем выполнения. 
Полученный список заданий должен быть отсортирован от заданий с наибольшим временем выполнения к заданиям с наименьшим временем выполнения.

Замечания:
- Невыполненные задания (не дошедшие до статуса "Выполнено") не учитываются.
- Когда исполнитель берет задание в работу, оно переходит в статус "В работе" (InProgress) и находится там до завершения работы. После чего переходит в статус "Выполнено" (Done).
  В любой момент времени задание может быть безвозвратно отменено - в этом случае оно перейдет в статус "Отменено" (Canceled).
- Нет разницы выполняется задание или подзадание.
- Выборка должна включать задания за все время.

Выборка должна содержать следующий набор полей:
- номер задания (task_number)
- заголовок задания (task_title)
- название статуса задания (status_name)
- email автора задания (author_email)
- email текущего исполнителя (assignee_email)
- дата и время создания задания (created_at)
- дата и время первого перехода в статус В работе (in_progress_at)
- дата и время выполнения задания (completed_at)
- количество дней, часов, минут и секнуд, которые задание находилось в работе - в формате "dd HH:mm:ss" (work_duration)

### Решение
```sql
   select t.number                                                  as task_number
        , t.title                                                   as task_title
        , ts.name                                                   as status_name
        , creators.email                                            as author_email
        , assigneers.email                                          as assignee_email
        , to_char(t.created_at, 'dd.MM.yyyy HH24:mi:ss')            as task_created_at
        , to_char(tl.at, 'dd.MM.yyyy HH24:mi:ss')                   as in_progress_at
        , to_char(t.completed_at, 'dd.MM.yyyy HH24:mi:ss')          as completed_at
        , to_char(t.completed_at - tl.at, 'dd.MM.yyyy HH24:mi:ss')  as work_duration
     from tasks t
left join task_statuses ts on ts.id = t.status
left join users creators on creators.id = t.created_by_user_id
left join users assigneers on assigneers.id = t.assigned_to_user_id
left join task_logs tl on tl.task_id = t.id
    where t.status = 4 -- Выполнено
      and tl.status = 3 -- В работе
    order by work_duration desc
    limit 100;
```

## Задание 2: Выборка для проверки вложенности
Задания могу быть простыми и составными. Составное задание содержит в себе дочерние - так получается иерархия заданий.
Глубина иерархии ограничено Н-уровнями, поэтому перед добавлением подзадачи к текущей задачи нужно понять, может ли пользователь добавить задачу уровнем ниже текущего или нет. Для этого нужно написать выборку для метода проверки перед добавлением подзадания, которая бы вернула уровень вложенности указанного задания и полный путь до него от родительского задания.

Замечания:
- ИД проверяемого задания передаем в sql как параметр _:parent_task_id_
- если задание _Е_ находится на 5м уровне, то путь должен быть "_//A/B/C/D/E_".

Выборка должна содержать:
- только 1 строку
- поле "Уровень задания" (level) - уровень указанного в параметре задания
- поле "Путь" (path)

### Решение
```sql
with recursive tree
                    as (
        select t.id               as task_id
             , t.parent_task_id   as parent_task_id
             , '//' || t.id::text as path
             , 1                  as level
          from tasks t
         where t.parent_task_id is null
         union all
        select t.id                         as task_id
             , t.parent_task_id             as parent_task_id
             , tt.path || '/' || t.id::text as path
             , tt.level + 1                 as level
          from tree tt
          join tasks t on t.parent_task_id = tt.task_id
    )
select tree.level as level
     , tree.path  as path
  from tree
 where tree.task_id = :parent_task_id;
```

## Задание 3 (на 10ку): Денормализация
Наш трекер задач пользуется популярностью и количество только активных задач перевалило уже за несколько миллионов. Продакт пришел с очередной юзер-стори:
```
Я как Диспетчер в списке активных задач всегда должен видеть задачу самого высокого уровня из цепочки отдельным полем

Требования:
1. Список активных задач включает в себя задачи со статусом "В работе"
2. Список должен быть отсортирован от самой новой задачи к самой старой
3. В списке должны быть поля:
  - Номер задачи (task_number)
  - Заголовок (task_title)
  - Номер родительской задачи (parent_task_number)
  - Заголовок родительской задачи (parent_task_title)
  - Номер самой первой задачи из цепочки (root_task_number)
  - Заголовок самой первой задачи из цепочки (root_task_title)
  - Email, на кого назначена задача (assigned_to_email)
  - Когда задача была создана (created_at)
 4. Должна быть возможность получить данные с пагинацией по N-строк (@limit, @offset)
```

Обсудив требования с лидом тебе прилетели 2 задачи:
1. Данных очень много и нужно денормализовать таблицу tasks
   Добавить в таблицу tasks поле `root_task_id bigint not null`
   Написать скрипт заполнения нового поля root_task_id для всей таблицы (если задача является рутом, то ее id должен совпадать с root_task_id)
2. Написать запрос получения данных для отображения в списке активных задач
   (!) Выяснилось, что дополнительно еще нужно возвращать идентификаторы задач, чтобы фронтенд мог сделать ссылки на них (т.е. добавить поля task_id, parent_task_id, root_task_id)

<details>
  <summary>FAQ</summary>

**Q: Что такое root_task_id?**

A: Например, есть задача с id=10 и parent_task_id=9, задача с id=9 имеет parent_task_id=8 и т.д. до задача id=1 имеет parent_task_id=null. Для всех этих задач root_task_id=1.

**Q: Не понял в каком формате нужен результат?**

Ожидаемый результат выполнения SQL-запроса:

| task_id | task_number | task_title | parent_task_id | parent_task_number | parent_task_title | root_task_id | root_task_number | root_task_title | assigned_to_email | created_at          |
|---------|-------------|------------|----------------|--------------------|-------------------|--------------|------------------|-----------------|-------------------|---------------------|
| 1       | A123        | Тест 123   | null           | null               | null              | 1            | A123             | Тест 123        | test@test.tt      | 01.01.2023 08:00:00 |
| 2       | B123        | B-тест     | 1              | A123               | Тест 123          | 1            | A123             | Тест 123        | user@test.tt      | 01.01.2023 11:00:00 |
| 3       | C123        | 123-тест   | 2              | B123               | B-тест            | 1            | A123             | Тест 123        | dev@test.tt       | 01.01.2023 11:10:00 |
| 10      | 1-2345      | New task   | null           | null               | null              | 10           | 1-2345           | New task        | test@test.tt      | 12.02.2024 11:00:00 |

**Q: Все это можно делать в одной миграции?**

А: Нет, каждая DDL операция - отдельная миграция, DML-операция тоже долзна быть в отдельной миграции.

</details>

### Скрипты миграций
```sql
alter table tasks
        add column if not exists root_task_id bigint not null DEFAULT 0;


with recursive tree
                   as (
        select t.id               as task_id
             , t.id               as root_task_id
             , t.parent_task_id   as parent_task_id
          from tasks t
         where t.parent_task_id is null
         union all
        select t.id                         as task_id
             , tt.root_task_id              as root_task_id
             , t.parent_task_id             as parent_task_id
          from tree tt
          join tasks t on t.parent_task_id = tt.task_id
    )
update tasks
set root_task_id = (select tt.root_task_id
                      from tree tt
                     where tt.task_id = tasks.id);
```

### Запрос выборки
```sql
    select t.id                                           as task_id
         , t.number                                       as task_number
         , t.title                                        as task_title
         , t.parent_task_id                               as parent_task_id
         , (select pt.number
              from tasks pt
             where pt.id = t.parent_task_id)              as parent_task_number
         , (select pt.title
              from tasks pt
             where pt.id = t.parent_task_id)              as parent_task_title
         , t.root_task_id                                 as root_task_id
         , (select rt.number
              from tasks rt
             where rt.id = t.root_task_id)                as root_task_number
         , (select rt.title
              from tasks rt
             where rt.id = t.root_task_id)                as root_task_title
         , u.email                                        as assigned_to_email
         , to_char(t.created_at, 'dd.MM.yyyy HH24:mi:ss') as created_at
      from tasks t
 left join users u on u.id = t.assigned_to_user_id
     where t.status = 3 -- В работе
     order by t.created_at desc
    offset :offset
     limit :limit;
```
