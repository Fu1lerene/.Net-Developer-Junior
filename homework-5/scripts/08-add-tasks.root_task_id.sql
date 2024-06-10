alter table tasks
        add column if not exists root_task_id bigint not null DEFAULT 0;