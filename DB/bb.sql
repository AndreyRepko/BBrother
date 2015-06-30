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


--
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

