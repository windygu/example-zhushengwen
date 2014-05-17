using System;
using System.Collections.Generic;
using System.Text;

///���ܣ������������ �¼�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.publish
{
    class cPublishControl : IDisposable 
    {

        //�������ļ��ˣ�ʵ��Ӧ�ð��ղɼ���ģʽ�����У����Խ��и��ּ��
        //������������� ���������ʱ����Զ��̷߳��������ܸо����ַ���ģʽ
        //ʵ���Բ��Ǻܴ����Ծ�����һ���������ܣ�������ϵͳ���������о�һ��
        //���õ�Ч��,��������������޸�.

        private cPublishManage m_PublishManage;

        public cPublishControl()
        {
            m_PublishManage = new cPublishManage();
        }

        ~cPublishControl()
        {
        }

        public cPublishManage PublishManage
        {
            get { return m_PublishManage; }
        }
        
        //���ӷ�������,���ڷ������ݣ�ͬʱ����������
        public void startPublish(cPublishTask pT)
        {
            m_PublishManage.AddPublishTask(pT );
        }

        //���ӷ�������,���ڷ�����ʱ�ɼ������ݣ�ͬʱ����������
        public void startSaveTempData(cPublishTask pT)
        {
            m_PublishManage.AddSaveTempDataTask(pT);
        }

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� Download �ĵ�ǰʵ��ʹ�õ�������Դ
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                 
                  
                }

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion

    }




}
