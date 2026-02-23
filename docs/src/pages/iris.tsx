import React from "react";
import Layout from "@theme/Layout";
import Link from "@docusaurus/Link";
import clsx from "clsx";
import styles from "/src/css/iris.module.css";

export default function IrisLanding() {
  return (
    <Layout
      title="IRIS — Inteligência Operacional"
      description="IRIS é a inteligência operacional da RUKIA. Não é chatbot. É orquestração contextual por domínio."
    >
      <main>

        {/* HERO */}
        <section className={styles.hero}>
          <div className="container">
            <div className={styles.heroInner}>
              <div className={styles.badge}>IA OPERACIONAL</div>

              <h1 className={styles.title}>
                IRIS não responde perguntas.
                <br />
                <span>Ela governa decisões.</span>
              </h1>

              <p className={styles.subtitle}>
                Inteligência contextual integrada ao fluxo real da operação.
                Orienta, bloqueia, alerta e explica — sempre respeitando
                contratos canônicos e permissões de domínio.
              </p>

              <div className={styles.buttons}>
                <Link
                  className="button button--primary button--lg"
                  to="/docs/agente-iris"
                >
                  Ver documentação técnica
                </Link>
                <Link
                  className="button button--secondary button--lg"
                  to="/docs/arquitetura/visao-geral"
                >
                  Ver arquitetura
                </Link>
              </div>
            </div>
          </div>
        </section>

        {/* DIFERENCIAL */}
        <section className={styles.sectionAlt}>
          <div className="container">
            <h2 className={styles.sectionTitle}>
              Não é um chatbot. É inteligência de fluxo.
            </h2>

            <div className={styles.grid}>
              <div className={styles.card}>
                <h3>Contextual</h3>
                <p>
                  IRIS entende o domínio, o estado da OP e o perfil do usuário
                  antes de responder.
                </p>
              </div>

              <div className={styles.card}>
                <h3>Governada</h3>
                <p>
                  Não inventa regra. Opera com base em contratos congelados e
                  decisões oficiais.
                </p>
              </div>

              <div className={styles.card}>
                <h3>Auditável</h3>
                <p>
                  Toda intervenção pode ser rastreada por evento e decisão.
                </p>
              </div>
            </div>
          </div>
        </section>

        {/* SPOTLIGHT */}
        <section className={styles.section}>
          <div className="container">
            <div className={styles.split}>
              <div>
                <h2 className={styles.sectionTitle}>
                  IRIS atua no momento do erro.
                </h2>
                <p className={styles.text}>
                  Se uma OP não pode avançar, IRIS explica o motivo.
                  Se um campo está bloqueado, ela indica a regra.
                  Se algo está inconsistente, ela aponta a origem.
                </p>
              </div>

              <div className={styles.panel}>
                <div className={styles.row}>IRIS, por que a OP está travada?</div>
                <div className={styles.row}>IRIS, o que falta para avançar?</div>
                <div className={styles.row}>IRIS, quem pode aprovar isso?</div>
              </div>
            </div>
          </div>
        </section>

      </main>
    </Layout>
  );
}