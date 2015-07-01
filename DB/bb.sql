--
-- PostgreSQL database dump
--
DROP DATABASE IF EXISTS bbrother;
DROP ROLE IF EXISTS bbAdmin;
CREATE DATABASE bbrother;

CREATE ROLE bbAdmin LOGIN
  ENCRYPTED PASSWORD 'md54689246406868d651879b6520c1bcee6'
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;
  
ALTER ROLE bbAdmin WITH PASSWORD 'qwerty';

\connect bbrother

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: event_log; Type: TABLE; Schema: public; Owner: bbrother; Tablespace: 
--

CREATE TABLE event_log (
    id bigint NOT NULL,
    event_time time with time zone,
    ip text,
    user_name text,
    event text,
    code integer
);


ALTER TABLE event_log OWNER TO bbAdmin;

--
-- Name: event_log_id_seq; Type: SEQUENCE; Schema: public; Owner: bbrother
--

CREATE SEQUENCE event_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE event_log_id_seq OWNER TO bbAdmin;

--
-- Name: event_log_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: bbrother
--

ALTER SEQUENCE event_log_id_seq OWNED BY event_log.id;


--
-- Name: id; Type: DEFAULT; Schema: public; Owner: bbrother
--

ALTER TABLE ONLY event_log ALTER COLUMN id SET DEFAULT nextval('event_log_id_seq'::regclass);


--
-- Data for Name: event_log; Type: TABLE DATA; Schema: public; Owner: bbrother
--

COPY event_log (id, event_time, ip, user_name, event, code) FROM stdin;
1	20:48:35.06113+03	no ip	\N	Server is starting now	\N
\.


--
-- Name: event_log_id_seq; Type: SEQUENCE SET; Schema: public; Owner: bbrother
--

SELECT pg_catalog.setval('event_log_id_seq', 1, true);


--
-- Name: id_pk; Type: CONSTRAINT; Schema: public; Owner: bbrother; Tablespace: 
--

ALTER TABLE ONLY event_log
    ADD CONSTRAINT id_pk PRIMARY KEY (id);
	

CREATE OR REPLACE FUNCTION pl_insert_alias_hist()
  RETURNS trigger AS
$BODY$
begin
   new.login_name = lower(new.login_name);
   return new;
end;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION pl_insert_alias_hist()
  OWNER TO bbAdmin;
  


CREATE OR REPLACE FUNCTION pg_log_insert_tr()
  RETURNS trigger AS
$BODY$
DECLARE
 al_id integer;
 al_hist_id integer;
 ex_id integer;
begin
  /* Тело функции */
  IF new.user_name<>'service' THEN
   SELECT alias_id, id INTO al_id,al_hist_id
     FROM alias_hist AS a
     WHERE ((a.date_end IS NULL) OR (a.date_end>current_timestamp)) AND (a.ip=new.ip) AND (a.login_name=lower(new.user_name));
   IF NOT FOUND THEN
     SELECT alias_id, id INTO al_id,al_hist_id
       FROM alias_hist AS a
       WHERE ((a.date_end IS NULL) OR (a.date_end>current_timestamp)) AND (a.ip=new.ip);
     IF NOT FOUND THEN
        INSERT INTO alias (name,comment) VALUES ('New user',(new.user_name::text)||','||(new.ip::text));
        INSERT INTO alias_hist (date_begin,ip,alias_id, login_name) VALUES (NEW.client_time,NEW.ip,currval('alias_id_seq'), lower(new.user_name));
       al_id:=currval('alias_id_seq');
       al_hist_id:=currval('alias_hist_id_seq');
     END IF;
   END IF;
   new.alias_hist_id=al_hist_id;
  ELSE
   new.alias_hist_id=0;
  END IF;
  new.user_name='';
  DELETE FROM log WHERE (event_id in (5,6)) and (alias_hist_id=al_hist_id);
  return new;
end;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION pg_log_insert_tr()
  OWNER TO bbAdmin;
  

CREATE TABLE event_type
(
  id serial NOT NULL,
  name text,
  comment text,
  CONSTRAINT pk_event_type PRIMARY KEY (id)
)
WITH (
  OIDS=TRUE
);
ALTER TABLE event_type OWNER TO bbAdmin;

COMMENT ON TABLE event_type
  IS 'Типы типов сообщений';

CREATE UNIQUE INDEX event_type_pk
  ON event_type
  USING btree
  (id);

CREATE TABLE event
(
  id serial NOT NULL,
  name text,
  type_id integer,
  comment text,
  CONSTRAINT pk_event PRIMARY KEY (id),
  CONSTRAINT fk_event_reference_event FOREIGN KEY (type_id)
      REFERENCES event_type (id)
      ON UPDATE RESTRICT ON DELETE RESTRICT
)
WITH (
  OIDS=TRUE
);

ALTER TABLE event
  OWNER TO bbAdmin;
COMMENT ON TABLE event
  IS 'Типы сообщений';

CREATE UNIQUE INDEX event_pk
  ON event
  USING btree
  (id);

CREATE INDEX reference_3_fk
  ON event
  USING btree
  (type_id);
  
CREATE TABLE alias
(
  id serial NOT NULL,
  name text NOT NULL,
  comment text,
  CONSTRAINT pk_alias PRIMARY KEY (id)
)
WITH (
  OIDS=TRUE
);
ALTER TABLE alias
  OWNER TO bbAdmin;
COMMENT ON TABLE alias
  IS 'Таблица алиасов Люди - ПК. Обладает временной характеристикой';

CREATE UNIQUE INDEX alias_pk
  ON alias
  USING btree
  (id);

CREATE TABLE alias_hist
(
  id serial NOT NULL,
  date_begin timestamp without time zone NOT NULL DEFAULT ('now'::text)::timestamp(6) with time zone,
  date_end timestamp without time zone,
  ip character varying(25) NOT NULL,
  alias_id integer NOT NULL,
  login_name character varying(50),
  CONSTRAINT pk_alias_hist PRIMARY KEY (id),
  CONSTRAINT fk_alias_reference_alias FOREIGN KEY (alias_id)
      REFERENCES alias (id)
      ON UPDATE RESTRICT ON DELETE CASCADE
)
WITH (
  OIDS=TRUE
);
ALTER TABLE alias_hist
  OWNER TO bbAdmin;
COMMENT ON TABLE alias_hist
  IS 'Таблица истории алиасов';

CREATE UNIQUE INDEX alias_hist_pk
  ON alias_hist
  USING btree
  (id);

CREATE INDEX reference_5_fk
  ON alias_hist
  USING btree
  (alias_id);


CREATE TRIGGER tr_alias_hist_change
  AFTER INSERT
  ON alias_hist
  FOR EACH ROW
  EXECUTE PROCEDURE pl_insert_alias_hist();

  
CREATE TABLE log
(
  id serial NOT NULL,
  event_id integer NOT NULL,
  alias_hist_id integer,
  server_time timestamp without time zone NOT NULL DEFAULT ('now'::text)::timestamp(6) with time zone,
  isshowing boolean NOT NULL DEFAULT true,
  isactive boolean NOT NULL DEFAULT true,
  description text,
  client_time timestamp(0) without time zone,
  route_type smallint,
  ip character varying(25),
  user_name text,
  CONSTRAINT pk_log PRIMARY KEY (id),
  CONSTRAINT fk_log_reference_alias_hist FOREIGN KEY (alias_hist_id)
      REFERENCES alias_hist (id) 
      ON UPDATE RESTRICT ON DELETE RESTRICT,
  CONSTRAINT fk_log_reference_event FOREIGN KEY (event_id)
      REFERENCES event (id) 
      ON UPDATE RESTRICT ON DELETE RESTRICT
)
WITH (
  OIDS=TRUE
);
ALTER TABLE log
  OWNER TO bbAdmin;
COMMENT ON TABLE log
  IS 'События системы';

CREATE INDEX log_lastuser_ind
  ON log
  USING btree
  (event_id, alias_hist_id)
  WHERE event_id = 5 OR event_id = 6;

CREATE UNIQUE INDEX log_pk
  ON log
  USING btree
  (id);

-- Index: reference_10_fk

-- DROP INDEX reference_10_fk;

CREATE INDEX reference_10_fk
  ON log
  USING btree
  (alias_hist_id);

-- Index: reference_4_fk

-- DROP INDEX reference_4_fk;

CREATE INDEX reference_4_fk
  ON log
  USING btree
  (event_id);


CREATE TRIGGER tr_log_insert
  BEFORE INSERT
  ON log
  FOR EACH ROW
  EXECUTE PROCEDURE pg_log_insert_tr();

COPY event_type (id, name, "comment") FROM stdin;
16	Системные события	Вход в сеанс Windows, смена установок времени и т.п.
17	Состояние настроек	Контроль получения, корректности, применения
18	Ошибки КЛК	Сбои, нарушения в работе программы и т.д.
19	Вторжения	Вероятное несанкционированное вмешательство пользователя
15	Подключение к серверу	Подсоединения и отсоединения от сервера 
20	Приватный режим	\N
\.

COPY event (id, name, type_id, "comment") FROM stdin;
7	Служебный 1	\N	\N
12	Служебный 2	\N	\N
1	Предупреждение	15	Подсоединение клиента
2	Попытка взлома	15	Отключение клиента от сервера
14	Ошибка востановления работы сервиса	19	\N
16	Ошибка запуска приложения на активной терминальной сессии	19	\N
0	Другое	\N	\N
18	Приватный режим запущен	20	\N
19	Приватный режим отключен	20	\N
10	Ручная смена настроек приложения	19	\N
11	Ручная смена настроек сервиса	19	\N
13	Восстановлена работа сервиса	18	\N
15	Запущено приложение на активной терминальной сесии	16	\N
3	Начало слежения	15	\N
20	Обновление файлов	16	\N
4	Окончание слежения	15	\N
17	Ошибка временного интервала	19	\N
5	Пользователь активен	16	\N
9	Пользователь ввел неправильный пароль для смены  настроек службы\r\n	19	\N
8	Пользователь ввел неправильный пароль для смены настроек приложения	19	\N
6	Пользователь неактивен	16	\N
21	Новое событие 1	\N	\N
22	Новое событие 2	\N	\N
23	Новое событие 3	\N	\N
24	Новое событие 4	\N	\N
25	Новое событие 5	\N	\N
26	Новое событие 6	\N	\N
\.


 
REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
REVOKE ALL ON SCHEMA public FROM bbAdmin;

GRANT ALL ON SCHEMA public TO bbAdmin;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

