--
-- PostgreSQL database dump
--

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
-- Name: info_log; Type: TABLE; Schema: public; Owner: bbrother_admin; Tablespace: 
--

CREATE TABLE info_log (
    id bigint NOT NULL,
    event_time time with time zone,
    ip text,
    user_name text,
    event text,
    code integer
);


ALTER TABLE info_log OWNER TO bbrother_admin;

--
-- Name: info_log_id_seq; Type: SEQUENCE; Schema: public; Owner: bbrother_admin
--

CREATE SEQUENCE info_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE info_log_id_seq OWNER TO bbrother_admin;

--
-- Name: info_log_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: bbrother_admin
--

ALTER SEQUENCE info_log_id_seq OWNED BY info_log.id;


--
-- Name: id; Type: DEFAULT; Schema: public; Owner: bbrother_admin
--

ALTER TABLE ONLY info_log ALTER COLUMN id SET DEFAULT nextval('info_log_id_seq'::regclass);


--
-- Data for Name: info_log; Type: TABLE DATA; Schema: public; Owner: bbrother_admin
--

COPY info_log (id, event_time, ip, user_name, event, code) FROM stdin;
1	20:48:35.06113+03	no ip	\N	Server is starting now	\N
\.


--
-- Name: info_log_id_seq; Type: SEQUENCE SET; Schema: public; Owner: bbrother_admin
--

SELECT pg_catalog.setval('info_log_id_seq', 1, true);


--
-- Name: id_pk; Type: CONSTRAINT; Schema: public; Owner: bbrother_admin; Tablespace: 
--

ALTER TABLE ONLY info_log
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

