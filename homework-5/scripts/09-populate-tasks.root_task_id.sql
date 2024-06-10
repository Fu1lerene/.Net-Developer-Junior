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