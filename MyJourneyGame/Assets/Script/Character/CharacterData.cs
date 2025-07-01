using UnityEngine;

/// <summary>
/// �L�����N�^�[�i�v���C���[��G�j�̃X�e�[�^�X�E�X�L���E�����ځE���ʉ��Ȃǂ�
/// �ꊇ�Ǘ����� ScriptableObject �f�[�^��`�B
/// Unity�G�f�B�^��ŌʃL�����N�^�[�̐ݒ�t�@�C�����쐬�ł���B
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// �g�p�\�ȃX�L���̎�ނ��`�i�U���E�񕜁E�o�t�E�f�o�t�Ȃǁj
    /// </summary>
    public enum SkillType
    {
        None,       // �Ȃ��i�X�L�����g�p�j
        Slash,      // �a���i�o�����ʂȂǁj
        Heal,       // �񕜃X�L��
        Fire,       // �Α����U���i�Ώ��j
        Buff,       // �����n
        Debuff,     // ��̉�
        HeavyBlow,  // �ꌂ��_���[�W
        MultiHit,   // �����q�b�g�U��
        Poison,     // �ł�t�^
        Bleed,      // �o����Ԃ�t�^
        Shield      // �h��A�b�v�E�V�[���h�t�^
    }

    [Header("�X�L���֘A")]
    public SkillType m_skillType;        // ���̃L�������g���X�L���̎��

    [Header("��{���")]
    public string m_characterName;       // �L�����N�^�[��
    public int m_maxHP = 100;            // �ő�HP
    public int m_maxSP = 30;             // �ő�SP

    [Header("�U����")]
    public int m_normalAttack;           // �ʏ�U���̊�{�_���[�W
    public int m_skillAttack;            // �X�L���U���̊�{�_���[�W
    public float m_addPowerMin;          // �ŏ������_���{���i��: 0.8�j
    public float m_addPowerMax;          // �ő僉���_���{���i��: 1.2�j

    [Header("�����ځi�\��X�v���C�g�j")]
    public Sprite m_iconNormal;          // �ʏ펞�̃A�C�R��
    public Sprite m_iconAttack;          // �U�����̃A�C�R��
    public Sprite m_iconLowHP;           // HP���Ⴂ���̃A�C�R��
    public Sprite m_iconDamaged;         // �_���[�W���󂯂��Ƃ��̃A�C�R��
    public Sprite m_iconSelected;        // �I�𒆃A�C�R��

    [Header("���ʉ�")]
    public AudioClip m_attackSE;         // �ʏ�U�����̌��ʉ�
    public AudioClip m_skillSE;          // �X�L���g�p���̌��ʉ�
}
