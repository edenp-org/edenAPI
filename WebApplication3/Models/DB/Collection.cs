using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    /// <summary>
    /// �ϼ��࣬��ʾ���ݿ��еĺϼ���
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// �ϼ�ID������������
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// �ϼ�����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �ϼ�����
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public long Code { get; set; }

        /// <summary>
        /// ������Code
        /// </summary>
        public long UCode { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        public string UName { get; set; }
    }
}